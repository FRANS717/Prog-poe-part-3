using PROG6221_V1.Controls;
using PROG6221_V1.Data;
using PROG6221_V1.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PROG6221_V1
{
    public partial class Form1 : Form
    {
        // --- EXISTING FIELDS (Preserved) ---
        private ChatbotEngine? bot;

        private Label? lblTitle;
        private Label? lblSubtitle;
        private Label? lblName;
        private TextBox? txtName;
        private Button? btnStart;
        private RichTextBox? rtbChat;
        private TextBox? txtInput;
        private Button? btnSend;

        // --- NEW PART 3 FIELDS ---
        private TabControl? tabMain;
        private TabPage? tabChatPage;
        private TabPage? tabTaskPage;
        private TabPage? tabQuizPage;
        private TabPage? tabLogPage;

        private TaskService? _taskService;
        private QuizManager? _quizManager;
        private ActivityLogger? _activityLogger;
        private IntentRecognizer? _intentRecognizer;

        private TaskControl? _taskControl;
        private QuizControl? _quizControl;
        private ActivityLogControl? _logControl;

        // --- CONSTRUCTOR ---
        public Form1()
        {
            InitializeComponent();

            // Initialize services FIRST (they don't need UI)
            _activityLogger = new ActivityLogger();
            _taskService = new TaskService(_activityLogger);
            _quizManager = new QuizManager(_activityLogger);
            _intentRecognizer = new IntentRecognizer(_activityLogger);

            // Build UI (includes new tabs)
            BuildInterface();

            // Test database connection silently
            try
            {
                DatabaseManager.Instance.TestConnection();
                _activityLogger.Log("System", "Database connection established.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database connection warning: {ex.Message}\n\nTask features will not work until database is configured.",
                                "Database Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _activityLogger.Log("System Error", $"Database connection failed: {ex.Message}");
            }
        }

        // --- BUILD INTERFACE (Modified to include TabControl) ---
        private void BuildInterface()
        {
            Text = "Cyber Shield - Matrix Mode";
            Size = new Size(1100, 750); // Wider to fit tabs
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.Black;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // --- TOP HEADER (Unchanged) ---
            lblTitle = new Label();
            lblTitle.Text = "CYBER SHIELD";
            lblTitle.ForeColor = Color.Lime;
            lblTitle.Font = new Font("Consolas", 28, FontStyle.Bold);
            lblTitle.Location = new Point(35, 20);
            lblTitle.AutoSize = true;
            Controls.Add(lblTitle);

            lblSubtitle = new Label();
            lblSubtitle.Text = "Matrix Security Awareness Terminal";
            lblSubtitle.ForeColor = Color.FromArgb(80, 255, 120);
            lblSubtitle.Font = new Font("Consolas", 10, FontStyle.Regular);
            lblSubtitle.Location = new Point(40, 72);
            lblSubtitle.AutoSize = true;
            Controls.Add(lblSubtitle);

            lblName = new Label();
            lblName.Text = "USER";
            lblName.ForeColor = Color.Lime;
            lblName.Font = new Font("Consolas", 10, FontStyle.Bold);
            lblName.Location = new Point(40, 112);
            lblName.AutoSize = true;
            Controls.Add(lblName);

            txtName = new TextBox();
            txtName.Location = new Point(105, 108);
            txtName.Size = new Size(220, 35);
            txtName.Font = new Font("Consolas", 11);
            txtName.BackColor = Color.FromArgb(5, 20, 5);
            txtName.ForeColor = Color.Gray;
            txtName.BorderStyle = BorderStyle.FixedSingle;
            txtName.Text = "Enter your name...";
            txtName.Enter += (s, e) =>
            {
                if (txtName.Text == "Enter your name...")
                {
                    txtName.Text = "";
                    txtName.ForeColor = Color.Lime;
                }
            };
            txtName.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    txtName.Text = "Enter your name...";
                    txtName.ForeColor = Color.Gray;
                }
            };
            Controls.Add(txtName);

            btnStart = new Button();
            btnStart.Text = "INITIALIZE";
            btnStart.Location = new Point(340, 105);
            btnStart.Size = new Size(130, 42);
            btnStart.BackColor = Color.FromArgb(15, 45, 15);
            btnStart.ForeColor = Color.Lime;
            btnStart.FlatStyle = FlatStyle.Flat;
            btnStart.FlatAppearance.BorderColor = Color.Lime;
            btnStart.FlatAppearance.BorderSize = 1;
            btnStart.Font = new Font("Consolas", 10, FontStyle.Bold);
            btnStart.Cursor = Cursors.Hand;
            btnStart.Click += BtnStart_Click;
            Controls.Add(btnStart);

            // --- MAIN TAB CONTROL (NEW) ---
            tabMain = new TabControl();
            tabMain.Location = new Point(20, 160);
            tabMain.Size = new Size(1050, 540);
            tabMain.BackColor = Color.Black;
            tabMain.ForeColor = Color.Lime;
            tabMain.Font = new Font("Consolas", 9, FontStyle.Bold);

            // Tab 1: Chat (Existing)
            tabChatPage = new TabPage("💬 CHAT");
            tabChatPage.BackColor = Color.Black;
            tabChatPage.ForeColor = Color.Lime;

            // Chat controls (moved into tab)
            rtbChat = new RichTextBox();
            rtbChat.Location = new Point(10, 10);
            rtbChat.Size = new Size(1010, 340);
            rtbChat.ReadOnly = true;
            rtbChat.BackColor = Color.FromArgb(0, 10, 0);
            rtbChat.ForeColor = Color.Lime;
            rtbChat.BorderStyle = BorderStyle.FixedSingle;
            rtbChat.Font = new Font("Consolas", 10);
            tabChatPage.Controls.Add(rtbChat);

            txtInput = new TextBox();
            txtInput.Location = new Point(10, 365);
            txtInput.Size = new Size(860, 40);
            txtInput.Font = new Font("Consolas", 11);
            txtInput.BackColor = Color.FromArgb(5, 20, 5);
            txtInput.ForeColor = Color.Lime;
            txtInput.BorderStyle = BorderStyle.FixedSingle;
            txtInput.Enabled = false;
            txtInput.KeyDown += TxtInput_KeyDown;
            tabChatPage.Controls.Add(txtInput);

            btnSend = new Button();
            btnSend.Text = "SEND";
            btnSend.Location = new Point(885, 362);
            btnSend.Size = new Size(135, 42);
            btnSend.BackColor = Color.FromArgb(15, 45, 15);
            btnSend.ForeColor = Color.Lime;
            btnSend.FlatStyle = FlatStyle.Flat;
            btnSend.FlatAppearance.BorderColor = Color.Lime;
            btnSend.FlatAppearance.BorderSize = 1;
            btnSend.Font = new Font("Consolas", 10, FontStyle.Bold);
            btnSend.Cursor = Cursors.Hand;
            btnSend.Enabled = false;
            btnSend.Click += BtnSend_Click;
            tabChatPage.Controls.Add(btnSend);

            tabMain.TabPages.Add(tabChatPage);

            // Tab 2: Tasks
            tabTaskPage = new TabPage("📋 TASKS");
            tabTaskPage.BackColor = Color.Black;
            _taskControl = new TaskControl(_taskService!);
            _taskControl.Dock = DockStyle.Fill;
            tabTaskPage.Controls.Add(_taskControl);
            tabMain.TabPages.Add(tabTaskPage);

            // Tab 3: Quiz
            tabQuizPage = new TabPage("🧠 QUIZ");
            tabQuizPage.BackColor = Color.Black;
            _quizControl = new QuizControl(_quizManager!);
            _quizControl.Dock = DockStyle.Fill;
            tabQuizPage.Controls.Add(_quizControl);
            tabMain.TabPages.Add(tabQuizPage);

            // Tab 4: Activity Log
            tabLogPage = new TabPage("📋 LOGS");
            tabLogPage.BackColor = Color.Black;
            _logControl = new ActivityLogControl(_activityLogger!);
            _logControl.Dock = DockStyle.Fill;
            tabLogPage.Controls.Add(_logControl);
            tabMain.TabPages.Add(tabLogPage);

            Controls.Add(tabMain);
        }

        // --- BTN START (Unchanged logic, but we now log it) ---
        private void BtnStart_Click(object? sender, EventArgs e)
        {
            string name = txtName!.Text.Trim();
            if (string.IsNullOrWhiteSpace(name) || name == "Enter your name...")
                name = "Friend";

            bot = new ChatbotEngine(name);

            rtbChat!.Clear();

            AddBotMessage(bot.GetAsciiArt());
            AddBotMessage("Access granted. Welcome, " + name + ".");
            AddBotMessage("Ask about phishing, passwords, safe browsing, scams, privacy, or 2FA.");
            AddBotMessage("Try: 'I am worried about scams', 'I am interested in privacy', or 'tell me more'.");
            AddBotMessage("\n--- NEW PART 3 COMMANDS ---");
            AddBotMessage("📌 'Add task: Buy groceries' | 'View my tasks' | 'Complete task 1'");
            AddBotMessage("🧠 'Start quiz' | 📋 'Show logs' | Type 'Help me' for all commands.");

            AudioPlayer.PlayGreeting("Audio.wav");

            txtInput!.Enabled = true;
            btnSend!.Enabled = true;
            txtInput.Focus();

            _activityLogger?.Log("System", $"User '{name}' initialized the chatbot.");
        }

        // --- TXT INPUT / SEND (Unchanged) ---
        private void BtnSend_Click(object? sender, EventArgs e)
        {
            ProcessUserInput();
        }

        private void TxtInput_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ProcessUserInput();
                e.SuppressKeyPress = true;
            }
        }

        // --- THE MODIFIED PROCESS USER INPUT (Routing added) ---
        private void ProcessUserInput()
        {
            if (bot == null)
            {
                MessageBox.Show("Initialize the system first.");
                return;
            }

            string userInput = txtInput!.Text.Trim();

            if (string.IsNullOrWhiteSpace(userInput))
            {
                AddBotMessage("Input required.");
                return;
            }

            AddUserMessage(userInput);

            // --- NEW: Route through IntentRecognizer FIRST ---
            var (intent, extractedData) = _intentRecognizer!.Parse(userInput);

            bool handled = false;
            string response = "";

            switch (intent)
            {
                case IntentRecognizer.Intent.AddTask:
                    // Extract task title (remove the command part)
                    string taskTitle = userInput;
                    // Simple extraction: remove common prefixes
                    string[] prefixes = { "add task", "create task", "make task", "new task", "remind me to", "i need to remember", "save this" };
                    foreach (var prefix in prefixes)
                    {
                        if (taskTitle.ToLower().StartsWith(prefix))
                        {
                            taskTitle = taskTitle.Substring(prefix.Length).Trim();
                            break;
                        }
                    }
                    if (string.IsNullOrWhiteSpace(taskTitle))
                    {
                        response = "Please specify what task you want to add. Example: 'Add task: Buy groceries'";
                    }
                    else
                    {
                        try
                        {
                            // Use the TaskService to add with current date as reminder default
                            _taskService!.AddTask(taskTitle, "Added via chat command", DateTime.Now.AddDays(1));
                            response = $"✅ Task '{taskTitle}' added successfully! View it in the Tasks tab.";
                            // Refresh task control
                            _taskControl?.RefreshTasks();
                        }
                        catch (Exception ex)
                        {
                            response = $"❌ Error adding task: {ex.Message}";
                        }
                    }
                    handled = true;
                    break;

                case IntentRecognizer.Intent.ViewTasks:
                    var tasks = _taskService!.GetAllTasks();
                    if (tasks.Count == 0)
                        response = "📭 You have no tasks. Add one with 'Add task: ...'";
                    else
                        response = $"📋 You have {tasks.Count} tasks. Open the 'Tasks' tab to view them in detail, or type 'Show logs' for recent activity.";
                    // Switch to task tab automatically? Optional but nice.
                    tabMain!.SelectedTab = tabTaskPage;
                    handled = true;
                    break;

                case IntentRecognizer.Intent.CompleteTask:
                    if (string.IsNullOrWhiteSpace(extractedData) || !int.TryParse(extractedData, out int completeId))
                    {
                        response = "Please specify the task ID. Example: 'Complete task 3'";
                    }
                    else
                    {
                        if (_taskService!.MarkComplete(completeId))
                        {
                            response = $"✅ Task {completeId} marked as completed!";
                            _taskControl?.RefreshTasks();
                        }
                        else
                        {
                            response = $"❌ Task {completeId} not found or already completed.";
                        }
                    }
                    handled = true;
                    break;

                case IntentRecognizer.Intent.DeleteTask:
                    if (string.IsNullOrWhiteSpace(extractedData) || !int.TryParse(extractedData, out int deleteId))
                    {
                        response = "Please specify the task ID. Example: 'Delete task 2'";
                    }
                    else
                    {
                        if (_taskService!.DeleteTask(deleteId))
                        {
                            response = $"🗑️ Task {deleteId} deleted.";
                            _taskControl?.RefreshTasks();
                        }
                        else
                        {
                            response = $"❌ Task {deleteId} not found.";
                        }
                    }
                    handled = true;
                    break;

                case IntentRecognizer.Intent.EditTask:
                    response = "✏️ To edit a task, go to the 'Tasks' tab, select the task, fill in the new details, and click 'EDIT'.";
                    tabMain!.SelectedTab = tabTaskPage;
                    handled = true;
                    break;

                case IntentRecognizer.Intent.StartQuiz:
                    _quizManager!.ResetQuiz();
                    _quizControl?.Refresh(); // Force UI update
                    tabMain!.SelectedTab = tabQuizPage;
                    response = "🧠 Quiz started! Go to the 'Quiz' tab to answer questions.";
                    handled = true;
                    break;

                case IntentRecognizer.Intent.ShowLogs:
                    _logControl?.RefreshLogs();
                    tabMain!.SelectedTab = tabLogPage;
                    response = "📋 Showing recent logs in the 'Logs' tab.";
                    handled = true;
                    break;

                case IntentRecognizer.Intent.ShowAllLogs:
                    // We don't have a separate all logs view, but we can increase display count
                    _logControl?.RefreshLogs(); // It shows based on internal counter (click 'Show More' for full)
                    tabMain!.SelectedTab = tabLogPage;
                    response = "📋 Logs displayed. Click 'SHOW MORE' in the Logs tab to see all entries.";
                    handled = true;
                    break;

                case IntentRecognizer.Intent.Help:
                    response = _intentRecognizer.GetHelpText();
                    handled = true;
                    break;
            }

            // If handled, show the response and skip the old chatbot engine
            if (handled)
            {
                AddBotMessage(response);
                txtInput.Clear();
                txtInput.Focus();
                return;
            }

            // --- FALLBACK: Existing ChatbotEngine (UNCHANGED) ---
            string fallbackResponse = bot.GetResponse(userInput);
            AddBotMessage(fallbackResponse);

            txtInput.Clear();
            txtInput.Focus();
        }

        // --- UI Helpers (Unchanged) ---
        private void AddUserMessage(string message)
        {
            rtbChat!.SelectionColor = Color.FromArgb(120, 255, 120);
            rtbChat.AppendText("USER > " + message + Environment.NewLine + Environment.NewLine);
            rtbChat.SelectionColor = Color.Lime;
        }

        private void AddBotMessage(string message)
        {
            rtbChat!.SelectionColor = Color.Lime;
            rtbChat.AppendText("SYSTEM > " + message + Environment.NewLine + Environment.NewLine);
            rtbChat.SelectionColor = Color.Lime;
            rtbChat.ScrollToCaret();
        }
    }
}