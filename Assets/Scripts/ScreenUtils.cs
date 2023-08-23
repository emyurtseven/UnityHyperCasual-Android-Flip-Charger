using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides screen utilities
/// </summary>
public static class ScreenUtils
{
    #region Fields

    // saved to support resolution changes
    static int screenWidth;
    static int screenHeight;

    // cached for efficient boundary checking
    static float screenLeft;
    static float screenRight;
    static float screenTop;
    static float screenBottom;
    static float screenMiddleVertical;
    static float screenMiddleHorizontal;
    static float screenZ;

    static Vector3 mousePosition;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the left edge of the screen at start in world coordinates 
    /// </summary>
    /// <value>left edge of the screen</value>
    public static float ScreenLeft
    {
        get
        {
            CheckScreenSizeChanged();
            return screenLeft;
        }
    }

    /// <summary>
    /// Gets the right edge of the screen at start in world coordinates
    /// </summary>
    /// <value>right edge of the screen</value>
    public static float ScreenRight
    {
        get
        {
            CheckScreenSizeChanged();
            return screenRight;
        }
    }

    /// <summary>
    /// Gets the top edge of the screen at start in world coordinates
    /// </summary>
    /// <value>top edge of the screen</value>
    public static float ScreenTop
    {
        get
        {
            CheckScreenSizeChanged();
            return screenTop;
        }
    }

    /// <summary>
    /// Gets the bottom edge of the screen at start in world coordinates
    /// </summary>
    /// <value>bottom edge of the screen</value>
    public static float ScreenBottom
    {
        get 
        {
            CheckScreenSizeChanged();
            return screenBottom; 
        }
    }

    public static Vector2 ScreenMiddle
    {
        get 
        {
            CheckScreenSizeChanged();
            return new Vector2(screenMiddleVertical, screenMiddleHorizontal); 
        }
    }

    public static float ScreenZ
    {
        get
        {
            return screenZ;
        }
    }

    public static Vector3 MousePosition
    {
        get
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = -Camera.main.transform.position.z;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            return mousePosition;
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Initializes the screen utilities
    /// </summary>
    public static void Initialize()
    {
        // save to support resolution changes
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        // save screen edges in world coordinates
        screenZ = -Camera.main.transform.localPosition.z;
        Vector3 lowerLeftCornerScreen = new Vector3(0, 0, screenZ);
        Vector3 upperRightCornerScreen = new Vector3(screenWidth, screenHeight, screenZ);
        Vector3 middlePointScreen = new Vector3(screenWidth / 2, screenHeight / 2, screenZ);

        Vector3 lowerLeftCornerWorld =
            Camera.main.ScreenToWorldPoint(lowerLeftCornerScreen);
        Vector3 upperRightCornerWorld =
            Camera.main.ScreenToWorldPoint(upperRightCornerScreen);

        Vector3 middlePointWorld = Camera.main.ScreenToWorldPoint(middlePointScreen);

        screenLeft = lowerLeftCornerWorld.x;
        screenRight = upperRightCornerWorld.x;
        screenTop = upperRightCornerWorld.y;
        screenBottom = lowerLeftCornerWorld.y;
        screenMiddleHorizontal = middlePointWorld.y;
        screenMiddleVertical = middlePointWorld.x;
    }

    public static float PixelSizeToWorld(float pixelSize)
    {
        return pixelSize * ((screenRight - screenLeft) / screenWidth);
    }

    /// <summary>
    /// Checks for screen size change
    /// </summary>
    static void CheckScreenSizeChanged()
    {
        if (screenWidth != Screen.width ||
            screenHeight != Screen.height)
        {
            Initialize();
        }
    }

    #endregion
}
