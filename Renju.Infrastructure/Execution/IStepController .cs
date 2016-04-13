using System.ComponentModel;

namespace Renju.Infrastructure.Execution
{
    public interface IStepController : INotifyPropertyChanged
    {
        int CurrentStep { get; }
        bool IsPaused { get; }
        bool PauseOnStart { get; set; }
        void Pause();
        void Resume();
        void StepForward(int steps);
        void StepBackward(int steps);
    }
}
