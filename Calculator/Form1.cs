using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Calculator
{
    public partial class Form1 : Form
    {
        private bool _dragging;
        private Point _dragStart;
        private List<string> tokens = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dragging = true;
                _dragStart = e.Location;
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
                Location = new Point(
                    Location.X + e.X - _dragStart.X,
                    Location.Y + e.Y - _dragStart.Y);
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }
        private static bool IsOperator(string s) =>
            s == "+" || s == "-" || s == "*" || s == "/";

        private void RefreshDisplay() =>
            richTextBox1.Text = string.Join(" ", tokens);


        private void Form1_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = "0";
        }

       
        private void NumberButton_Click(object sender, EventArgs e)
        {
            string digit = ((Button)sender).Tag.ToString();
            AppendToken(digit);
        }

        private void DecimalButton_Click(object sender, EventArgs e)
        {
            if (tokens.Count == 0 || IsOperator(tokens.Last()))
            {
                tokens.Add("0.");
            }
            else if (!tokens.Last().Contains('.'))
            {
                tokens[tokens.Count - 1] += ".";
            }
            RefreshDisplay();
        }

        private void AppendToken(string digit)
        {
            if (tokens.Count == 0 || IsOperator(tokens.Last()))
            {
                tokens.Add(digit == "0" ? "0" : digit);
            }
            else
            {
                string last = tokens.Last();
                if (last == "0" && digit != ".")
                    tokens[tokens.Count - 1] = digit;
                else
                    tokens[tokens.Count - 1] = last + digit;
            }
            RefreshDisplay();
        }


        private void OperatorButton_Click(object sender, EventArgs e)
        {
            string op = ((Button)sender).Tag.ToString();

            if (tokens.Count == 0)
            {
                MessageBox.Show("Enter a number first.");
                return;
            }

            if (IsOperator(tokens.Last()))
            {
                tokens[tokens.Count - 1] = op;
            }
            else
            {
                tokens.Add(op);
            }
            RefreshDisplay();
        }


        private void EqualsButton_Click(object sender, EventArgs e)
        {
            if (tokens.Count == 0 || IsOperator(tokens.Last()))
            {
                MessageBox.Show("Enter a number first.");
                return;
            }
            Evaluate();
        }

        private void Evaluate()
        {
            try
            {
                var working = new List<string>(tokens);

                int i = 0;
                while (i < working.Count)
                {
                    if (i > 0 && i + 1 < working.Count &&
                        (working[i] == "*" || working[i] == "/"))
                    {
                        double left = double.Parse(working[i - 1]);
                        double right = double.Parse(working[i + 1]);
                        double res = working[i] == "*" ? left * right : left / right;

                        working[i - 1] = res.ToString("G15");
                        working.RemoveAt(i);
                        working.RemoveAt(i);
                        i = 0;
                    }
                    else i++;
                }

                i = 0;
                while (i < working.Count)
                {
                    if (i > 0 && i + 1 < working.Count &&
                        (working[i] == "+" || working[i] == "-"))
                    {
                        double left = double.Parse(working[i - 1]);
                        double right = double.Parse(working[i + 1]);
                        double res = working[i] == "+" ? left + right : left - right;

                        working[i - 1] = res.ToString("G15");
                        working.RemoveAt(i);
                        working.RemoveAt(i);
                        i = 0;
                    }
                    else i++;
                }

                tokens.Clear();
                tokens.Add(working[0]);
                richTextBox1.Text = working[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show("Calculation error: " + ex.Message);
                tokens.Clear();
                richTextBox1.Text = "0";
            }
        }


        private void ClearButton_Click(object sender, EventArgs e)
        {
            tokens.Clear();
            richTextBox1.Text = "0";
        }

        private void BackspaceButton_Click(object sender, EventArgs e)
        {
            if (tokens.Count == 0) return;

            string last = tokens.Last();
            if (last.Length <= 1 || (last.Length == 2 && last.StartsWith("-")))
                tokens.RemoveAt(tokens.Count - 1);
            else
                tokens[tokens.Count - 1] = last.Substring(0, last.Length - 1);

            RefreshDisplay();
        }


        private void NegateButton_Click(object sender, EventArgs e)
        {
            if (tokens.Count == 0 || IsOperator(tokens.Last())) return;

            string last = tokens.Last();
            tokens[tokens.Count - 1] = last.StartsWith("-")
                ? last.Substring(1)
                : "-" + last;
            RefreshDisplay();
        }

        private void ExitLabel_Click(object sender, EventArgs e) =>
            Application.Exit();

       
    }
}