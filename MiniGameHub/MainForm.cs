using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace MiniGameHub
{
    public class MainForm : Form
    {
        private Panel pnlMenu;
        private Panel pnlTicTacToe;
        private Panel pnlQuiz;
        private Panel pnlPuzzle;
        private Panel currentPanel;

        // Fonts and Colors
        private Font titleFont = new Font("Segoe UI", 24, FontStyle.Bold);
        private Font menuFont = new Font("Segoe UI", 14, FontStyle.Regular);
        private Font gameFont = new Font("Segoe UI", 12, FontStyle.Regular);
        private Color bgColor = Color.FromArgb(240, 248, 255); // AliceBlue
        private Color btnColor = Color.FromArgb(70, 130, 180); // SteelBlue
        private Color btnTextColor = Color.White;

        public MainForm()
        {
            this.Text = "Mini Game Hub";
            this.Size = new Size(600, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = bgColor;
            
            InitializePanels();
            ShowPanel(pnlMenu);
        }

        private void InitializePanels()
        {
            pnlMenu = CreateBasePanel();
            pnlTicTacToe = CreateBasePanel();
            pnlQuiz = CreateBasePanel();
            pnlPuzzle = CreateBasePanel();

            this.Controls.Add(pnlMenu);
            this.Controls.Add(pnlTicTacToe);
            this.Controls.Add(pnlQuiz);
            this.Controls.Add(pnlPuzzle);

            BuildMenuPanel();
            BuildTicTacToePanel();
            BuildQuizPanel();
            BuildPuzzlePanel();
        }

        private Panel CreateBasePanel()
        {
            return new Panel
            {
                Dock = DockStyle.Fill,
                Visible = false,
                BackColor = bgColor
            };
        }

        private void ShowPanel(Panel pnl)
        {
            if (currentPanel != null) currentPanel.Visible = false;
            currentPanel = pnl;
            currentPanel.Visible = true;
        }

        private Button CreateStyledButton(string text, int x, int y, int width, int height)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, height),
                Font = menuFont,
                BackColor = btnColor,
                ForeColor = btnTextColor,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private Label CreateTitleLabel(string text)
        {
            return new Label
            {
                Text = text,
                Font = titleFont,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 80,
                ForeColor = Color.DarkSlateBlue
            };
        }

        private Label CreateInstructionLabel(string text)
        {
            return new Label
            {
                Text = text,
                Font = gameFont,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Top = 80,
                Width = 600,
                Height = 30,
                ForeColor = Color.DimGray
            };
        }

        private Button CreateBackButton()
        {
            var btn = CreateStyledButton("Back to Menu", 215, 530, 150, 40);
            btn.BackColor = Color.IndianRed;
            btn.Click += (s, e) => ShowPanel(pnlMenu);
            return btn;
        }

        // --- MENU PANEL ---
        private void BuildMenuPanel()
        {
            pnlMenu.Controls.Add(CreateTitleLabel("Mini Game Hub"));
            
            var btnTicTacToe = CreateStyledButton("1. Tic-Tac-Toe", 190, 150, 200, 50);
            btnTicTacToe.Click += (s, e) => { ResetTicTacToe(); ShowPanel(pnlTicTacToe); };
            
            var btnQuiz = CreateStyledButton("2. Quiz Game", 190, 220, 200, 50);
            btnQuiz.Click += (s, e) => { StartQuiz(); ShowPanel(pnlQuiz); };
            
            var btnPuzzle = CreateStyledButton("3. Sliding Puzzle", 190, 290, 200, 50);
            btnPuzzle.Click += (s, e) => { ResetPuzzle(); ShowPanel(pnlPuzzle); };

            pnlMenu.Controls.Add(btnTicTacToe);
            pnlMenu.Controls.Add(btnQuiz);
            pnlMenu.Controls.Add(btnPuzzle);
        }

        // --- TIC TAC TOE ---
        private Button[,] tttButtons = new Button[3, 3];
        private bool isXTurn = true;
        private Label lblTttStatus;

        private void BuildTicTacToePanel()
        {
            pnlTicTacToe.Controls.Add(CreateTitleLabel("Tic-Tac-Toe"));
            pnlTicTacToe.Controls.Add(CreateInstructionLabel("Get 3 in a row to win!"));

            lblTttStatus = new Label
            {
                Text = "Player X's Turn",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Top = 110,
                Width = 600,
                Height = 40
            };
            pnlTicTacToe.Controls.Add(lblTttStatus);

            Panel grid = new Panel { Size = new Size(300, 300), Location = new Point(140, 160) };
            for(int r = 0; r < 3; r++)
            {
                for(int c = 0; c < 3; c++)
                {
                    var btn = new Button
                    {
                        Size = new Size(95, 95),
                        Location = new Point(c * 100, r * 100),
                        Font = new Font("Segoe UI", 36, FontStyle.Bold),
                        BackColor = Color.White,
                        FlatStyle = FlatStyle.Flat
                    };
                    btn.Click += TttButton_Click;
                    tttButtons[r, c] = btn;
                    grid.Controls.Add(btn);
                }
            }
            pnlTicTacToe.Controls.Add(grid);
            
            var btnReset = CreateStyledButton("Reset Game", 215, 480, 150, 40);
            btnReset.BackColor = Color.SeaGreen;
            btnReset.Click += (s, e) => ResetTicTacToe();
            pnlTicTacToe.Controls.Add(btnReset);

            pnlTicTacToe.Controls.Add(CreateBackButton());
        }

        private void TttButton_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            if (btn.Text != "") return;

            btn.Text = isXTurn ? "X" : "O";
            btn.ForeColor = isXTurn ? Color.Crimson : Color.MediumBlue;
            
            if (CheckTttWin())
            {
                lblTttStatus.Text = (isXTurn ? "Player X" : "Player O") + " Wins!";
                EnableTttGrid(false);
            }
            else if (CheckTttDraw())
            {
                lblTttStatus.Text = "It's a Draw!";
            }
            else
            {
                isXTurn = !isXTurn;
                lblTttStatus.Text = (isXTurn ? "Player X" : "Player O") + "'s Turn";
            }
        }

        private bool CheckTttWin()
        {
            // rows & cols
            for(int i = 0; i < 3; i++)
            {
                if (tttButtons[i, 0].Text != "" && tttButtons[i, 0].Text == tttButtons[i, 1].Text && tttButtons[i, 1].Text == tttButtons[i, 2].Text) return true;
                if (tttButtons[0, i].Text != "" && tttButtons[0, i].Text == tttButtons[1, i].Text && tttButtons[1, i].Text == tttButtons[2, i].Text) return true;
            }
            // diag
            if (tttButtons[0, 0].Text != "" && tttButtons[0, 0].Text == tttButtons[1, 1].Text && tttButtons[1, 1].Text == tttButtons[2, 2].Text) return true;
            if (tttButtons[0, 2].Text != "" && tttButtons[0, 2].Text == tttButtons[1, 1].Text && tttButtons[1, 1].Text == tttButtons[2, 0].Text) return true;
            
            return false;
        }

        private bool CheckTttDraw()
        {
            foreach(var btn in tttButtons) if(btn.Text == "") return false;
            return true;
        }

        private void EnableTttGrid(bool enable)
        {
            foreach(var btn in tttButtons) btn.Enabled = enable;
        }

        private void ResetTicTacToe()
        {
            isXTurn = true;
            lblTttStatus.Text = "Player X's Turn";
            foreach(var btn in tttButtons)
            {
                btn.Text = "";
                btn.Enabled = true;
            }
        }

        // --- QUIZ GAME ---
        private Label lblQuestion;
        private RadioButton[] rbAnswers = new RadioButton[4];
        private int currentQuestionIndex = 0;
        private int quizScore = 0;
        private Button btnNextQuiz;

        private class QuizItem
        {
            public string Question { get; set; }
            public string[] Answers { get; set; }
            public int CorrectIndex { get; set; }
        }

        private List<QuizItem> questions = new List<QuizItem>
        {
            new QuizItem { Question = "What is the primary color of a ripe banana?", Answers = new[] { "Red", "Blue", "Yellow", "Green" }, CorrectIndex = 2 },
            new QuizItem { Question = "How many legs does a spider have?", Answers = new[] { "6", "8", "10", "12" }, CorrectIndex = 1 },
            new QuizItem { Question = "Which of the following is a type of fruit?", Answers = new[] { "Carrot", "Potato", "Apple", "Broccoli" }, CorrectIndex = 2 },
            new QuizItem { Question = "What do bees collect from flowers?", Answers = new[] { "Nectar", "Water", "Dirt", "Leaves" }, CorrectIndex = 0 },
            new QuizItem { Question = "Which ocean is the largest?", Answers = new[] { "Atlantic", "Indian", "Arctic", "Pacific" }, CorrectIndex = 3 }
        };

        private void BuildQuizPanel()
        {
            pnlQuiz.Controls.Add(CreateTitleLabel("Quiz Game"));
            pnlQuiz.Controls.Add(CreateInstructionLabel("Answer the multiple choice questions."));

            lblQuestion = new Label
            {
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Top = 130,
                Width = 500,
                Height = 60,
                Left = 40
            };
            pnlQuiz.Controls.Add(lblQuestion);

            Panel pnlAnswers = new Panel { Top = 200, Left = 140, Width = 300, Height = 200 };
            for(int i = 0; i < 4; i++)
            {
                var rb = new RadioButton
                {
                    Font = gameFont,
                    Top = i * 40,
                    Left = 20,
                    Width = 260,
                    AutoSize = false,
                    Cursor = Cursors.Hand
                };
                rbAnswers[i] = rb;
                pnlAnswers.Controls.Add(rb);
            }
            pnlQuiz.Controls.Add(pnlAnswers);

            btnNextQuiz = CreateStyledButton("Next", 215, 420, 150, 40);
            btnNextQuiz.BackColor = Color.SeaGreen;
            btnNextQuiz.Click += BtnNextQuiz_Click;
            pnlQuiz.Controls.Add(btnNextQuiz);

            pnlQuiz.Controls.Add(CreateBackButton());
        }

        private void StartQuiz()
        {
            currentQuestionIndex = 0;
            quizScore = 0;
            btnNextQuiz.Text = "Next";
            btnNextQuiz.Visible = true;
            foreach (var rb in rbAnswers) rb.Visible = true;
            LoadQuestion();
        }

        private void LoadQuestion()
        {
            if (currentQuestionIndex < questions.Count)
            {
                var q = questions[currentQuestionIndex];
                lblQuestion.Text = $"Q{currentQuestionIndex + 1}: {q.Question}";
                for(int i = 0; i < 4; i++)
                {
                    rbAnswers[i].Text = q.Answers[i];
                    rbAnswers[i].Checked = false;
                }
            }
            else
            {
                lblQuestion.Text = $"Quiz Completed! Score: {quizScore}/{questions.Count}";
                foreach (var rb in rbAnswers) rb.Visible = false;
                btnNextQuiz.Visible = false;
            }
        }

        private void BtnNextQuiz_Click(object sender, EventArgs e)
        {
            bool answered = false;
            for(int i = 0; i < 4; i++)
            {
                if(rbAnswers[i].Checked)
                {
                    answered = true;
                    if (i == questions[currentQuestionIndex].CorrectIndex) quizScore++;
                    break;
                }
            }

            if (!answered)
            {
                MessageBox.Show("Please select an answer.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            currentQuestionIndex++;
            LoadQuestion();
        }

        // --- PUZZLE GAME ---
        private Button[,] pzlButtons = new Button[3, 3];
        private int emptyRow = 2;
        private int emptyCol = 2;

        private void BuildPuzzlePanel()
        {
            pnlPuzzle.Controls.Add(CreateTitleLabel("Sliding Puzzle"));
            pnlPuzzle.Controls.Add(CreateInstructionLabel("Arrange tiles from 1-8. Click a tile to move it."));

            Panel grid = new Panel { Size = new Size(300, 300), Location = new Point(140, 140) };
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    var btn = new Button
                    {
                        Size = new Size(95, 95),
                        Location = new Point(c * 100, r * 100),
                        Font = new Font("Segoe UI", 24, FontStyle.Bold),
                        BackColor = btnColor,
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Cursor = Cursors.Hand
                    };
                    btn.FlatAppearance.BorderSize = 2;
                    btn.Click += PzlButton_Click;
                    btn.Tag = new Point(r, c);
                    pzlButtons[r, c] = btn;
                    grid.Controls.Add(btn);
                }
            }
            pnlPuzzle.Controls.Add(grid);

            var btnShuffle = CreateStyledButton("Shuffle / Restart", 190, 460, 200, 40);
            btnShuffle.BackColor = Color.SeaGreen;
            btnShuffle.Click += (s, e) => ResetPuzzle();
            pnlPuzzle.Controls.Add(btnShuffle);

            pnlPuzzle.Controls.Add(CreateBackButton());
        }

        private void PzlButton_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            if (btn.Text == "") return;

            Point pt = (Point)btn.Tag;
            int r = pt.X;
            int c = pt.Y;

            // Check if adjacent to empty space
            if ((Math.Abs(r - emptyRow) == 1 && c == emptyCol) || (Math.Abs(c - emptyCol) == 1 && r == emptyRow))
            {
                pzlButtons[emptyRow, emptyCol].Text = btn.Text;
                pzlButtons[emptyRow, emptyCol].Visible = true;
                
                btn.Text = "";
                btn.Visible = false;
                emptyRow = r;
                emptyCol = c;

                if (CheckPuzzleWin())
                {
                    MessageBox.Show("Congratulations! You solved the puzzle!", "Puzzle Solved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void ResetPuzzle()
        {
            List<string> nums = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "" };
            
            // To ensure 100% solvability without complex parity logic when generating randomly:
            // We can start with a solved state and make random valid moves.
            int eR = 2, eC = 2;
            string[,] state = new string[3,3] 
            {
                { "1", "2", "3" },
                { "4", "5", "6" },
                { "7", "8", "" }
            };

            Random rnd = new Random();
            int shuffleMoves = 100;
            for(int i = 0; i < shuffleMoves; i++)
            {
                List<Point> validMoves = new List<Point>();
                if (eR > 0) validMoves.Add(new Point(eR - 1, eC));
                if (eR < 2) validMoves.Add(new Point(eR + 1, eC));
                if (eC > 0) validMoves.Add(new Point(eR, eC - 1));
                if (eC < 2) validMoves.Add(new Point(eR, eC + 1));

                Point move = validMoves[rnd.Next(validMoves.Count)];
                state[eR, eC] = state[move.X, move.Y];
                state[move.X, move.Y] = "";
                eR = move.X;
                eC = move.Y;
            }

            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    pzlButtons[r, c].Text = state[r, c];
                    if (state[r, c] == "")
                    {
                        pzlButtons[r, c].Visible = false;
                        emptyRow = r;
                        emptyCol = c;
                    }
                    else
                    {
                        pzlButtons[r, c].Visible = true;
                    }
                }
            }
        }

        private bool CheckPuzzleWin()
        {
            string[] expected = { "1", "2", "3", "4", "5", "6", "7", "8", "" };
            int index = 0;
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    if (pzlButtons[r, c].Text != expected[index]) return false;
                    index++;
                }
            }
            return true;
        }
    }
}
