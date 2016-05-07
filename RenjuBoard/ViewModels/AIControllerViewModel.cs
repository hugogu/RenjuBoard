﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows.Input;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Renju.Core;
using Renju.Infrastructure;
using Renju.Infrastructure.AI;
using Renju.Infrastructure.Events;
using Renju.Infrastructure.Execution;
using Renju.Infrastructure.Model;
using Renju.Infrastructure.Model.Extensions;

namespace RenjuBoard.ViewModels
{
    public class AIControllerViewModel : DisposableModelBase
    {
        private readonly ObservableCollection<PieceLine> _previewLines = new ObservableCollection<PieceLine>();
        private readonly VirtualGameBoard<BoardPoint> _resolvingBoard;

        public AIControllerViewModel(IStepController aiStepController, IDropResolver dropResolver, IGameBoard<IReadOnlyBoardPoint> gameBoard, IEventAggregator eventAggregator)
        {
            Debug.Assert(aiStepController != null);
            Debug.Assert(aiStepController.CurrentStep == 0);
            _resolvingBoard = new VirtualGameBoard<BoardPoint>(gameBoard.Size, BoardPoint.CreateIndexBasedFactory(gameBoard.Size));
            eventAggregator.GetEvent<ResolvingBoardEvent>().Subscribe(OnResolvingBoard);
            PauseAICommand = new DelegateCommand(() => aiStepController.Pause(), () => !aiStepController.IsPaused);
            ContinueAICommand = new DelegateCommand(() => aiStepController.Resume(), () => aiStepController.IsPaused);
            NextAIStepComand = new DelegateCommand(() => aiStepController.StepForward(1), () => aiStepController.IsPaused);
            ClearPreviewLinesCommand = new DelegateCommand(() => _previewLines.Clear());
            PreviewLinesCommand = new DelegateCommand<IReadOnlyBoardPoint>(point =>
            {
                _previewLines.Clear();
                _previewLines.AddRange(gameBoard.GetRowsFromPoint(point, true));
            }, p => Options.ShowPreviewLine);
            AutoDispose(aiStepController.ObserveProperty(() => aiStepController.IsPaused).ObserveOnDispatcher().Subscribe((_ =>
            {
                PauseAICommand.RaiseCanExecuteChanged();
                ContinueAICommand.RaiseCanExecuteChanged();
                NextAIStepComand.RaiseCanExecuteChanged();
            })));
        }

        [Dependency]
        public IGamePlayer AIPlayer { get; internal set; }

        [Dependency]
        public GameOptions Options { get; internal set; }

        public DelegateCommand PauseAICommand { get; private set; }

        public DelegateCommand NextAIStepComand { get; private set; }

        public DelegateCommand ContinueAICommand { get; private set; }

        public ICommand PreviewLinesCommand { get; private set; }

        public ICommand ClearPreviewLinesCommand { get; private set; }

        public IEnumerable<PieceLine> PreviewLines
        {
            get { return _previewLines; }
        }

        public IEnumerable<IReadOnlyBoardPoint> ResolvingPoints
        {
            get { return _resolvingBoard.Points; }
        }

        private void OnResolvingBoard(IReadBoardState<IReadOnlyBoardPoint> board)
        {
            foreach (var showingPoint in _resolvingBoard.Points)
            {
                var virtualPoint = board == null ? null : board[showingPoint.Position];
                if (virtualPoint is VirtualBoardPoint && Options.ShowAISteps)
                {
                    showingPoint.Index = virtualPoint.Index;
                    showingPoint.Status = virtualPoint.Status;
                }
                else if (showingPoint.Index.HasValue)
                {
                    showingPoint.ResetToEmpty();
                }
            }
        }
    }
}
