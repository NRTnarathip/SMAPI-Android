using Netcode;
using StardewModdingAPI.Framework.StateTracking.Comparers;
using System.Collections.Generic;
using System.Reflection;

namespace StardewModdingAPI.Framework.StateTracking.FieldWatchers
{
    /// <summary>A watcher which detects changes to a net list field.</summary>
    /// <typeparam name="TValue">The list value type.</typeparam>
    internal class NetListWatcher<TValue> : BaseDisposableWatcher, ICollectionWatcher<TValue>
        where TValue : class, INetObject<INetSerializable>
    {
        /*********
        ** Fields
        *********/
        /// <summary>The field being watched.</summary>
        private readonly NetList<TValue, NetRef<TValue>> Field;

        /// <summary>The pairs added since the last reset.</summary>
        private readonly ISet<TValue> AddedImpl = new HashSet<TValue>(new ObjectReferenceComparer<TValue>());

        /// <summary>The pairs removed since the last reset.</summary>
        private readonly ISet<TValue> RemovedImpl = new HashSet<TValue>(new ObjectReferenceComparer<TValue>());


        /*********
        ** Accessors
        *********/
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public bool IsChanged => this.AddedImpl.Count > 0 || this.RemovedImpl.Count > 0;

        /// <inheritdoc />
        public IEnumerable<TValue> Added => this.AddedImpl;

        /// <inheritdoc />
        public IEnumerable<TValue> Removed => this.RemovedImpl;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="name">A name which identifies what the watcher is watching, used for troubleshooting.</param>
        /// <param name="field">The field to watch.</param>

        private void hookField(int index, NetRef<TValue> field)
        {
            if (!(field == null))
            {
                field.fieldChangeVisibleEvent += delegate (NetRef<TValue> f, TValue oldValue, TValue newValue)
                {
                    OnElementChanged(Field, index, oldValue, newValue);
                };
            }
        }

        private void hookArray(NetArray<TValue, NetRef<TValue>> array)
        {
            for (int i = 0; i < array.Count; i++)
            {
                hookField(i, array.Fields[i]);
            }
        }
        readonly NetRef<NetArray<TValue, NetRef<TValue>>> privateArrayNetRef;

        public NetListWatcher(string name, NetList<TValue, NetRef<TValue>> field)
        {
            this.Name = name;
            this.Field = field;

            //original
            //field.OnElementChanged += this.OnElementChanged;
            //field.OnArrayReplaced += this.OnArrayReplaced;

            //fix fix
            //get array in var field
            var arrayInField = field.GetType().GetField("array",
                BindingFlags.Instance | BindingFlags.NonPublic).GetValue(field);
            privateArrayNetRef = (NetRef<NetArray<TValue, NetRef<TValue>>>)arrayInField;

            //hook current array
            hookArray(privateArrayNetRef.Value);
            //check if new array & we just hook to new array again
            privateArrayNetRef.fieldChangeVisibleEvent += PrivateArrayNetRef_fieldChangeVisibleEvent;
        }
        void PrivateArrayNetRef_fieldChangeVisibleEvent(NetRef<NetArray<TValue, NetRef<TValue>>> field, NetArray<TValue, NetRef<TValue>> oldValue, NetArray<TValue, NetRef<TValue>> newValue)
        {
            if (newValue != null)
                hookArray(newValue);
            OnArrayReplaced(Field, oldValue, newValue);
        }

        /// <inheritdoc />
        public void Reset()
        {
            this.AddedImpl.Clear();
            this.RemovedImpl.Clear();
        }

        /// <inheritdoc />
        public void Update()
        {
            this.AssertNotDisposed();
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            if (!this.IsDisposed)
            {
                //this.Field.OnElementChanged -= this.OnElementChanged;
                //this.Field.OnArrayReplaced -= this.OnArrayReplaced;
            }
            //fix fix
            privateArrayNetRef.fieldChangeVisibleEvent -= PrivateArrayNetRef_fieldChangeVisibleEvent;

            base.Dispose();
        }


        /*********
        ** Private methods
        *********/
        /// <summary>A callback invoked when the value list is replaced.</summary>
        /// <param name="list">The net field whose values changed.</param>
        /// <param name="oldValues">The previous list of values.</param>
        /// <param name="newValues">The new list of values.</param>
        private void OnArrayReplaced(NetList<TValue, NetRef<TValue>> list, IList<TValue> oldValues, IList<TValue> newValues)
        {
            ISet<TValue> oldSet = new HashSet<TValue>(oldValues, new ObjectReferenceComparer<TValue>());
            ISet<TValue> changed = new HashSet<TValue>(newValues, new ObjectReferenceComparer<TValue>());

            foreach (TValue value in oldSet)
            {
                if (!changed.Contains(value))
                    this.Remove(value);
            }
            foreach (TValue value in changed)
            {
                if (!oldSet.Contains(value))
                    this.Add(value);
            }
        }

        /// <summary>A callback invoked when an entry is replaced.</summary>
        /// <param name="list">The net field whose values changed.</param>
        /// <param name="index">The list index which changed.</param>
        /// <param name="oldValue">The previous value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnElementChanged(NetList<TValue, NetRef<TValue>> list, int index, TValue? oldValue, TValue? newValue)
        {
            this.Remove(oldValue);
            this.Add(newValue);
        }

        /// <summary>Track an added item.</summary>
        /// <param name="value">The value that was added.</param>
        private void Add(TValue? value)
        {
            if (value == null)
                return;

            if (this.RemovedImpl.Contains(value))
            {
                this.AddedImpl.Remove(value);
                this.RemovedImpl.Remove(value);
            }
            else
                this.AddedImpl.Add(value);
        }

        /// <summary>Track a removed item.</summary>
        /// <param name="value">The value that was removed.</param>
        private void Remove(TValue? value)
        {
            if (value == null)
                return;

            if (this.AddedImpl.Contains(value))
            {
                this.AddedImpl.Remove(value);
                this.RemovedImpl.Remove(value);
            }
            else
                this.RemovedImpl.Add(value);
        }
    }
}
