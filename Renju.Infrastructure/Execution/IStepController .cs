namespace Renju.Infrastructure.Execution
{
    using System.ComponentModel;
    using Model;

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
