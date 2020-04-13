namespace GarageDoorService.Shared
{
    using System.Threading.Tasks;

    public interface IGarageDoorService
    {
        bool IsInitialized { get; }
        Task<bool> Initialize();
        Task OpenLeftGarageDoor();
        Task OpenRightGarageDoor();
    }
}
