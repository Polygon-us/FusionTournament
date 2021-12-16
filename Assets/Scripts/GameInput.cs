using Fusion;

public struct GameInput : INetworkInput
{
    #region Information

    public uint button;

    public const uint FORWARD = 1 << 2;
    public const uint BACKWARD = 1 << 3;
    public const uint LEFT = 1 << 4;
    public const uint RIGHT = 1 << 5;
   
    #endregion

    public bool IsUp(uint button)
    {
        return IsDown(button) == false;
    }

    public bool IsDown(uint button)
    {
        return (this.button & button) == button;
    }
}