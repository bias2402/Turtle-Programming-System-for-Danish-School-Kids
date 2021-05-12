using UnityEngine;

public static class Options {
    public static int volume;
    
    public static Sprite background;

    public static int avatarIndex = 0;

    public static void SetOptions(int vol) {
        volume = vol;
    }

    public static void SetAvatar(int index) {
        avatarIndex = index;
    }

    public static void SetBackground(Sprite newBackground) {
        background = newBackground;
    }
}