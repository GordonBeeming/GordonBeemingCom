// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
  // Loop through each anchor tag on the page
  $('a').each(function () {
    var a = new RegExp('/' + window.location.host + '/');
    // Check if it's an external link
    if (!a.test(this.href)) {
      // If the link contains only text
      if ($(this).children().length == 0) {
        $(this).append(" <i class='fas fa-external-link-alt'></i>");
      }

      // Store the original link
      var originalLink = this.href;

      // Encode the original link
      var encodedLink = encodeURIComponent(originalLink);

      // Change the href to redirect to the local page /external with the encoded original link as a query string
      this.href = "/external?link=" + encodedLink;
      this.target = "_blank"
      if (this.title.length > 0) {
        this.title = this.title + " | This is an external link, and will open in a new tab.";
      } else {
        this.title = "This is an external link, and will open in a new tab.";
      }
    }
  });
});