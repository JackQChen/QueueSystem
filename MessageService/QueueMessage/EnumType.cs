
namespace QueueMessage
{
    public enum ClientType
    {
        Service = 0,
        CallSystem = 1,
        CallClient = 2,
        SoundPlayer = 3,
        LEDDisplay = 4,
        ScreenDisplay = 5,
        QueueClient = 6
    }

    public enum ClientQueryType
    {
        Request = 0,
        Response = 1
    }

    public enum ClientChangedType
    {
        Add = 0,
        Remove = 1
    }

    public enum Operate
    {
        Pause = 0,
        Resume = 1,
        Reset = 2
    }

}
