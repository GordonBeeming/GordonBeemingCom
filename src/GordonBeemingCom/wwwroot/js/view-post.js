// #region checkImageSize

function checkImageSize() {
  var postWidth = $("article .container").width();

  $("article .container img").each(function (index, item) {
    $(item).attr("height", "");
    var width = $(item).attr("rel");
    if (width === undefined || width === "") {
      width = $(item).width();
      $(item).attr("rel", width);
    }
    width = parseInt(width);
    if (postWidth < width) {
      $(item).animate({ width: postWidth + "px" });
    }
  });
}

var checkImageSizeTimeout;
function checkImageSize_withDelay() {
  clearTimeout(checkImageSizeTimeout);
  checkImageSizeTimeout = setTimeout(checkImageSize, 250);
}

// #endregion

// #region checkIframeSize

function checkIframeSize() { 
  var postWidth = $("article .container").width();

  $("article .container iframe").each(function (index, item) {
    var isngnonbindable = $(item).attr("ng-non-bindable");
    if (typeof isngnonbindable !== 'undefined' && isngnonbindable !== false) {
      return;
    }
    var rel = $(item).attr("rel");
    if (rel === undefined || rel === "") {
      var width = parseInt($(item).width());
      var height = parseInt($(item).height());
      var rel = width + "|" + height;
      $(item).attr("rel", rel);
    }
    var relValue = $(item).attr("rel").split("|");
    var iframeWidth = parseInt(relValue[0]);
    var iframeHeight = parseInt(relValue[1]);
    var setWidthTo = Math.max(postWidth, iframeWidth);
    var heightShouldBe = parseInt(iframeHeight * (1.00 * setWidthTo / iframeWidth));
    $(item).animate({ width: setWidthTo + "px", height: heightShouldBe + "px" });
  });
}

var checkIframeSizeTimeout;
function checkIframeSize_withDelay() {
  clearTimeout(checkIframeSizeTimeout);
  checkIframeSizeTimeout = setTimeout(checkIframeSize, 250);
}

// #endregion

$(document).ready(function () {
  $("img.lazyload").lazy({
    afterLoad: function (element) {
      if (typeof checkImageSize_withDelay === "function") {
        checkImageSize_withDelay();
      }
    }
  });
  $(window).resize(function () {
    checkImageSize_withDelay();
    checkIframeSize_withDelay();
  });
  checkImageSize_withDelay();
  checkIframeSize_withDelay();
});
checkImageSize_withDelay();
checkIframeSize_withDelay();

