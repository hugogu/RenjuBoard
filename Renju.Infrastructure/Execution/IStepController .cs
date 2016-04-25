using System.ComponentModel;
using Renju.Infrastructure.Model;

namespace Renju.Infrastructure.Execution
{
    public interface IStepController : INotifyPropertyChanged
    {
        int CurrentStep { get; }
        bool IsPaused { get; }
        GameOptions Options { set; }
        void Pause();
        void Resume();
        void StepForward(int steps);
        void StepBackward(int steps);
    }
}
