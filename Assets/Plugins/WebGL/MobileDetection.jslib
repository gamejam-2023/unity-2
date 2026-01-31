mergeInto(LibraryManager.library, {
  
  // Detect if running on iOS (iPhone/iPad) via Safari or WebView
  IsiOSMobile: function() {
    var userAgent = navigator.userAgent || navigator.vendor || window.opera;
    
    // Check for iOS
    var isiOS = /iPad|iPhone|iPod/.test(userAgent) && !window.MSStream;
    
    // Also check for iPad on iOS 13+ which reports as Mac
    var isMacWithTouch = (navigator.platform === 'MacIntel' && navigator.maxTouchPoints > 1);
    
    // Additional check for iOS WebView
    var isiOSWebView = /(iPhone|iPod|iPad).*AppleWebKit(?!.*Safari)/i.test(userAgent);
    
    var result = isiOS || isMacWithTouch || isiOSWebView;
    
    console.log('[MobileDetection] IsiOSMobile check - userAgent: ' + userAgent);
    console.log('[MobileDetection] isiOS: ' + isiOS + ', isMacWithTouch: ' + isMacWithTouch + ', isiOSWebView: ' + isiOSWebView);
    console.log('[MobileDetection] IsiOSMobile result: ' + result);
    
    return result ? 1 : 0;
  },
  
  // Detect if running on Android mobile browser
  IsAndroidMobile: function() {
    var userAgent = navigator.userAgent || navigator.vendor || window.opera;
    var result = /android/i.test(userAgent);
    
    console.log('[MobileDetection] IsAndroidMobile check - result: ' + result);
    
    return result ? 1 : 0;
  },
  
  // Combined check for any mobile browser
  IsMobileBrowser: function() {
    var userAgent = navigator.userAgent || navigator.vendor || window.opera;
    
    // iOS detection
    var isiOS = /iPad|iPhone|iPod/.test(userAgent) && !window.MSStream;
    var isMacWithTouch = (navigator.platform === 'MacIntel' && navigator.maxTouchPoints > 1);
    var isiOSWebView = /(iPhone|iPod|iPad).*AppleWebKit(?!.*Safari)/i.test(userAgent);
    
    // Android detection  
    var isAndroid = /android/i.test(userAgent);
    
    // General mobile detection
    var isMobile = /Mobile|webOS|BlackBerry|IEMobile|Opera Mini/i.test(userAgent);
    
    // Touch capability as additional signal
    var hasTouch = ('ontouchstart' in window) || (navigator.maxTouchPoints > 0);
    
    // Mobile viewport detection
    var isSmallScreen = (window.innerWidth <= 1024 && hasTouch);
    
    var result = isiOS || isMacWithTouch || isiOSWebView || isAndroid || isMobile || isSmallScreen;
    
    console.log('[MobileDetection] IsMobileBrowser comprehensive check:');
    console.log('  userAgent: ' + userAgent);
    console.log('  isiOS: ' + isiOS + ', isMacWithTouch: ' + isMacWithTouch);
    console.log('  isAndroid: ' + isAndroid + ', isMobile: ' + isMobile);
    console.log('  hasTouch: ' + hasTouch + ', isSmallScreen: ' + isSmallScreen);
    console.log('  RESULT: ' + result);
    
    return result ? 1 : 0;
  },
  
  // Check if running in Safari browser
  IsSafariBrowser: function() {
    var userAgent = navigator.userAgent;
    var isSafari = /^((?!chrome|android).)*safari/i.test(userAgent);
    
    console.log('[MobileDetection] IsSafariBrowser: ' + isSafari);
    
    return isSafari ? 1 : 0;
  },
  
  // Force enable touch events for iOS Safari PWA mode
  EnableTouchEvents: function() {
    console.log('[MobileDetection] EnableTouchEvents called');
    
    // Ensure touch events work in fullscreen/PWA mode on iOS Safari
    document.addEventListener('touchstart', function(e) {}, { passive: true });
    document.addEventListener('touchmove', function(e) {}, { passive: true });
    document.addEventListener('touchend', function(e) {}, { passive: true });
    
    // Prevent default touch behaviors that might interfere
    var canvas = document.querySelector('#unity-canvas');
    if (canvas) {
      canvas.style.touchAction = 'none';
      canvas.addEventListener('touchstart', function(e) { e.preventDefault(); }, { passive: false });
      canvas.addEventListener('touchmove', function(e) { e.preventDefault(); }, { passive: false });
      
      console.log('[MobileDetection] Canvas touch events configured');
    }
  },
  
  // Get detailed device info for debugging
  GetMobileDeviceInfo: function() {
    var info = {
      userAgent: navigator.userAgent,
      platform: navigator.platform,
      maxTouchPoints: navigator.maxTouchPoints,
      innerWidth: window.innerWidth,
      innerHeight: window.innerHeight,
      devicePixelRatio: window.devicePixelRatio,
      touchSupported: 'ontouchstart' in window
    };
    
    var infoStr = JSON.stringify(info);
    console.log('[MobileDetection] Device info: ' + infoStr);
    
    var bufferSize = lengthBytesUTF8(infoStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(infoStr, buffer, bufferSize);
    return buffer;
  }
  
});
