using Unity.Mathematics;

public struct AnimationName
{
    public const int Attack1 = 0;
    public const int Attack2 = 1;
    public const int AttackRanged = 2;
    public const int Death = 3;
    public const int Falling = 4;
    public const int Idle = 5;
    public const int Walk = 6;
}

public struct AnimationClipDataBaked
{
    public float TextureOffset;
    public float TextureRange;
    public float OnePixelOffset;
    public int TextureWidth;

    public float AnimationLength;
    //public bool Looping;
}