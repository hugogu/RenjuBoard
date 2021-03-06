﻿namespace Renju.Infrastructure.Model
{
    using System;

    [Serializable]
    public class GameOptions : ModelBase
    {
        private bool _showLinesOnBoard = false;
        private bool _showPointWeight = true;
        private bool _showAISteps = true;
        private bool _showPreviewLine = true;
        private bool _showCalibrationLabels = true;
        private bool _isAITimeLimited = true;
        private bool _steppingAI = false;
        private int _aiStepTimeLimit = 20000;
        private int _aiTimeLimit = 500000;

        public bool ShowLinesOnBoard
        {
            get { return _showLinesOnBoard; }
            set { SetProperty(ref _showLinesOnBoard, value); }
        }

        public bool ShowPointWeight
        {
            get { return _showPointWeight; }
            set { SetProperty(ref _showPointWeight, value); }
        }

        public bool ShowAISteps
        {
            get { return _showAISteps; }
            set { SetProperty(ref _showAISteps, value); }
        }

        public bool ShowPreviewLine
        {
            get { return _showPreviewLine; }
            set { SetProperty(ref _showPreviewLine, value); }
        }

        public bool ShowCalibrationLabels
        {
            get { return _showCalibrationLabels; }
            set { SetProperty(ref _showCalibrationLabels, value); }
        }

        public bool SteppingAI
        {
            get { return _steppingAI; }
            set { SetProperty(ref _steppingAI, value); }
        }

        public bool IsAITimeLimited
        {
            get { return _isAITimeLimited; }
            set
            {
                SetProperty(ref _isAITimeLimited, value);
                OnPropertyChanged(() => AIStepTimeSpan);
                OnPropertyChanged(() => AITotalTimeSpan);
            }
        }

        public int AIStepTimeLimit
        {
            get { return _aiStepTimeLimit; }
            set
            {
                SetProperty(ref _aiStepTimeLimit, value);
                OnPropertyChanged(() => AIStepTimeSpan);
            }
        }

        public int AITimeLimit
        {
            get { return _aiTimeLimit; }
            set
            {
                SetProperty(ref _aiTimeLimit, value);
                OnPropertyChanged(() => AITotalTimeSpan);
            }
        }

        public TimeSpan AITotalTimeSpan
        {
            get { return IsAITimeLimited ? TimeSpan.FromMilliseconds(AITimeLimit) : TimeSpan.MaxValue; }
        }

        public TimeSpan AIStepTimeSpan
        {
            get { return IsAITimeLimited ? TimeSpan.FromMilliseconds(AIStepTimeLimit) : TimeSpan.MaxValue; }
        }
    }
}
