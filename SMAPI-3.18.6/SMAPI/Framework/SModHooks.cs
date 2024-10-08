using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace StardewModdingAPI.Framework
{
    /// <summary>Invokes callbacks for mod hooks provided by the game.</summary>
    internal class SModHooks : DelegatingModHooks
    {
        /*********
        ** Fields
        *********/
        /// <summary>A callback to invoke before <see cref="Game1.newDayAfterFade"/> runs.</summary>
        private readonly Action BeforeNewDayAfterFade;

        /// <summary>Writes messages to the console.</summary>
        private readonly IMonitor Monitor;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="parent">The underlying hooks to call by default.</param>
        /// <param name="beforeNewDayAfterFade">A callback to invoke before <see cref="Game1.newDayAfterFade"/> runs.</param>
        /// <param name="monitor">Writes messages to the console.</param>
        public SModHooks(ModHooks parent, Action beforeNewDayAfterFade, IMonitor monitor)
            : base(parent)
        {
            this.BeforeNewDayAfterFade = beforeNewDayAfterFade;
            this.Monitor = monitor;
        }

        /// <inheritdoc />
        public override void OnGame1_NewDayAfterFade(Action action)
        {
            this.BeforeNewDayAfterFade();
            action();
        }

        /// <inheritdoc />
        public override Task StartTask(Task task, string id)
        {
            var st = Stopwatch.StartNew();
            this.Monitor.Log($"Synchronizing '{id}' task...");
            task.RunSynchronously();
            this.Monitor.Log($"   taskID: {id} complete, use time: {st.Elapsed.TotalSeconds} seconds");
            st.Stop();
            return task;
        }

        /// <inheritdoc />
        public override Task<T> StartTask<T>(Task<T> task, string id)
        {
            var st = Stopwatch.StartNew();
            this.Monitor.Log($"Synchronizing '{id}' task...");
            task.RunSynchronously();
            st.Stop();
            this.Monitor.Log($"   taskID: {id} complete, use time: {st.Elapsed.TotalMilliseconds} seconds");
            return task;
        }
    }
}
