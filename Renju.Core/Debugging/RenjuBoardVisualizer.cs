using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using Microsoft.VisualStudio.DebuggerVisualizers;
using Renju.Controls;
using Renju.Infrastructure;
using Renju.Infrastructure.Model;

namespace Renju.Core.Debugging
{
    public class RenjuBoardVisualizer : DialogDebuggerVisualizer
    {
        private static readonly IEnumerable<ResourceDictionary> boardResources;

        static RenjuBoardVisualizer()
        {
            var mainAssembly = Assembly.Load("RenjuBoard");
            boardResources = mainAssembly.TryFindResourceDictionaries(key => key.StartsWith("themes/"));
            Debug.Assert(boardResources.Any());
        }

        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            if (windowService == null)
                throw new ArgumentNullException("windowService");
            if (objectProvider == null)
                throw new ArgumentNullException("objectProvider");

            var data = objectProvider.GetObject();
            var board = data as IReadBoardState<IReadOnlyBoardPoint>;
            if (board != null)
                ShowsBoard(windowService, board);
            var line = data as PieceLine;
            if (line != null)
                ShowLines(windowService, new[] { line });

            // TODO: Adding DebuggerVisualizerAttribute onto IEnumerable<PieceLine>
            var lines = data as IEnumerable<PieceLine>;
            if (lines != null && lines.Any())
                ShowLines(windowService, lines);
        }

        private void ShowLines(IDialogVisualizerService windowService, IEnumerable<PieceLine> lines)
        {
            var gamePanel = new GameBoardPanel()
            {
                Size = lines.First().Board.Size,
                Lines = lines,
                FontFamily = new FontFamily("Consola")
            };
            ShowsGamePanel(windowService, gamePanel);
        }

        private void ShowsBoard(IDialogVisualizerService windowService, IReadBoardState<IReadOnlyBoardPoint> board)
        {
            var gamePanel = new GameBoardPanel()
            {
                Size = board.Size,
                ItemsSource = board.Points,
                FontFamily = new FontFamily("Consola")
            };
            ShowsGamePanel(windowService, gamePanel);
        }

        private void ShowsGamePanel(IDialogVisualizerService windowService, GameBoardPanel gamePanel)
        {
            foreach (var resource in boardResources)
                gamePanel.Resources.MergedDictionaries.Add(resource);

            using (var form = new Form { Width = 400, Height = 400, Text = "Renju Board Debugger Visualizer" })
            {
                form.Controls.Add(new ElementHost()
                {
                    Child = gamePanel,
                    Dock = DockStyle.Fill,
                    TabStop = false,
                });
                windowService.ShowDialog(form);
            }
        }
    }
}
