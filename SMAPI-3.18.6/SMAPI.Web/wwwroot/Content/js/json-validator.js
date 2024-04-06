/* globals $ */

var smapi = smapi || {};

/**
 * Manages the logic for line range selections.
 * @param {int} maxLines The maximum number of lines in the content.
 */
smapi.LineNumberRange = function (maxLines) {
    var self = this;

    /**
     * @var {int} minLine The first line in the selection, or null if no lines selected.
     */
    self.minLine = null;

    /**
     * @var {int} maxLine The last line in the selection, or null if no lines selected.
     */
    self.maxLine = null;

    /**
     * Parse line numbers from a URL hash.
     * @param {string} hash the URL hash to parse.
     */
    self.parseFromUrlHash = function (hash) {
        self.minLine = null;
        self.maxLine = null;

        // parse hash
        var hashParts = hash.match(/^#L(\d+)(?:-L(\d+))?$/);
        if (!hashParts || hashParts.length <= 1)
            return;

        // extract min/max lines
        self.minLine = parseInt(hashParts[1]);
        self.maxLine = parseInt(hashParts[2]) || self.minLine;
    };

    /**
     * Generate a URL hash for the current line range.
     * @returns {string} The generated URL hash.
     */
    self.buildHash = function () {
        if (!self.minLine)
            return "";
        else if (self.minLine === self.maxLine)
            return "#L" + self.minLine;
        else
            return "#L" + self.minLine + "-L" + self.maxLine;
    }

    /**
     * Get a list of all selected lines.
     * @returns {Array<int>} The selected line numbers.
     */
    self.getLinesSelected = function () {
        // format
        if (!self.minLine)
            return [];

        var lines = [];
        for (var i = self.minLine; i <= self.maxLine; i++)
            lines.push(i);
        return lines;
    };

    return self;
};

/**
 * UI logic for the JSON validator page.
 * @param {string} urlFormat The URL format for a file, with $schemaName and $id placeholders.
 * @param {string} fileId The file ID for the content being viewed, if any.
 */
smapi.jsonValidator = function (urlFormat, fileId) {
    /**
     * The original content element.
     */
    var originalContent = $("#raw-content").clone();

    /**
     * The currently highlighted lines.
     */
    var selection = new smapi.LineNumberRange();

    /**
     * Rebuild the syntax-highlighted element.
     */
    var formatCode = function () {
        // reset if needed
        $(".sunlight-container").replaceWith(originalContent.clone());

        // apply default highlighting
        Sunlight.highlightAll({
            lineHighlight: selection.getLinesSelected()
        });

        // fix line links
        $(".sunlight-line-number-margin a").each(function () {
            var link = $(this);
            var lineNumber = parseInt(link.text());
            link
                .attr("id", "L" + lineNumber)
                .attr("href", "#L" + lineNumber)
                .removeAttr("name")
                .data("line-number", lineNumber);
        });
    };

    /**
     * Scroll the page so the selected range is visible.
     */
    var scrollToRange = function () {
        if (!selection.minLine)
            return;

        var targetLine = Math.max(1, selection.minLine - 5);
        $("#L" + targetLine).get(0).scrollIntoView();
    };

    /**
     * Initialize the JSON validator page.
     */
    var init = function () {
        var input = $("#input");

        // set initial code formatting
        selection.parseFromUrlHash(location.hash);
        formatCode();
        scrollToRange();

        // update code formatting on hash change
        $(window).on("hashchange", function () {
            selection.parseFromUrlHash(location.hash);
            formatCode();
            scrollToRange();
        });

        // change format
        $("#output #format").on("change", function () {
            var schemaName = $(this).val();
            location.href = urlFormat.replace("$schemaName", schemaName).replace("$id", fileId);
        });

        if (input.length) {
            // upload form
            smapi.fileUpload({
                chooseFileLink: $("#choose-file-link"),
                chooseFileInput: $("#inputFile"),
                contentArea: input,
                submitButton: $("#submit")
            });
        }
    };
    init();
};
