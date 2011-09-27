using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Sudoku
{
    public partial class Sudoku : Form
    {
        public Sudoku()
        {
            InitializeComponent();
            newPuzzle();
        }

        private void digitBoxes_Enter(object sender, EventArgs e)
        {
            ((MaskedTextBox)sender).SelectionStart = 0;
            ((MaskedTextBox)sender).SelectionLength = 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            newPuzzle();
            button2.Enabled = true;
            button3.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (byte i = 0; i < 9; ++i)
            {
                for (byte j = 0; j < 9; ++j)
                {
                    if (digitBoxes[i][j].Enabled && (digitBoxes[i][j].Text.Length < 1 || digitBoxes[i][j].Text[0] != board[i][j]))
                    {
                        MessageBox.Show(this, "Row " + (i + 1) + " Col " + (j + 1) + " is wrong!", "Sudoku");
                        digitBoxes[i][j].Focus();
                        return;
                    }
                }
            }
            MessageBox.Show("Congratulation, you win!", "Sudoku");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (byte i = 0; i < 9; ++i)
            {
                for (byte j = 0; j < 9; ++j)
                {
                    if (digitBoxes[i][j].Enabled)
                    {
                        digitBoxes[i][j].Text = "" + board[i][j];
                        digitBoxes[i][j].Enabled = false;
                    }
                }
            }
            button2.Enabled = false;
            button3.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            char[][] board = new char[9][];
            for(byte i = 0; i < 9; ++i)
            {
                board[i] = new char[9];
                for(byte j = 0; j < 9; ++j)
                {
                    if(digitBoxes2[i][j].Text.Length > 0)
                    {
                        board[i][j] = digitBoxes2[i][j].Text[0];
                    }
                    else
                    {
                        board[i][j] = '\0';
                    }
                }
            }
            if (!solve(board, 0, 0))
            {
                MessageBox.Show("No solution!");
                return;
            }
            for (byte i = 0; i < 9; ++i)
            {
                for (byte j = 0; j < 9; ++j)
                {
                    digitBoxes2[i][j].Text = "" + board[i][j];
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            for (byte i = 0; i < 9; ++i)
            {
                for (byte j = 0; j < 9; ++j)
                {
                    digitBoxes2[i][j].Text = "";
                }
            }
        }

        private ArrayList getValidDigits(char[][] board, byte row, byte col)
        {
            ArrayList digits = new ArrayList();
            for (char i = '1'; i <= '9'; ++i)
            {
                digits.Add(i);
            }
            // check rows and columns
            for (byte i = 0; i < 9; ++i)
            {
                if (i != col && board[row][i] > 0)
                    digits.Remove(board[row][i]);
                if (i != row && board[i][col] > 0)
                    digits.Remove(board[i][col]);
            }
            // check sub-box
            byte r = (byte)(row / 3 * 3);
            byte c = (byte)(col / 3 * 3);
            for (byte i = r; i < r + 3; ++i)
            {
                for (byte j = c; j < c + 3; ++j)
                {
                    if (i == row || j == col) continue;
                    if (board[i][j] > 0)
                        digits.Remove(board[i][j]);
                }
            }
            return digits;
        }

        private bool solve(char[][] board, byte row, byte col)
        {
            if (row == 9) return true;
            ArrayList digits = getValidDigits(board, row, col);
            if (board[row][col] > 0)
            {
                if(digits.Contains(board[row][col]))
                    return col == 8 ? solve(board, (byte)(row + 1), 0) : solve(board, row, (byte)(col + 1));
                else return false;
            }
            for(byte i = 0; i < digits.Count; ++i)
            {
                board[row][col] = (char)digits[i];
                if (col == 8 ? solve(board, (byte)(row + 1), 0) : solve(board, row, (byte)(col + 1)))
                {
                    return true;
                }
            }
            board[row][col] = '\0';
            return false;
        }

        private bool generate()
        {
            Random r = new Random();
            ArrayList digits = new ArrayList() { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            board = new char[9][];
            for (byte i = 0; i < 9; ++i)
            {
                board[i] = new char[9];
                for (byte j = 0; j < 9; ++j)
                {
                    board[i][j] = '\0';
                }
            }
            // initialize a random row
            for (byte i = 8; i > 1; --i)
            {
                byte p = (byte)r.Next(i);
                object temp = digits[i];
                digits[i] = digits[p];
                digits[p] = temp;
            }
            for (byte i = 0; i < 9; ++i)
            {
                board[0][i] = (char)digits[i];
            }
            // also initialize a random sub-box
            digits.Remove(board[0][0]);
            digits.Remove(board[0][1]);
            digits.Remove(board[0][2]);
            for (byte i = 5; i > 1; --i)
            {
                byte p = (byte)r.Next(i);
                object temp = digits[i];
                digits[i] = digits[p];
                digits[p] = temp;
            }
            for (byte i = 0; i < 6; ++i)
            {
                board[i / 3 + 1][i % 3] = (char)digits[i];
            }
            return solve(board, 0, 0);
        }

        private void newPuzzle()
        {
            Random r = new Random();
            generate();
            byte[] randoms = new byte[81];
            for (byte i = 0; i < 81; ++i)
            {
                randoms[i] = i;
            }
            // generate random blank positions
            for (byte i = 80; i > 1; --i)
            {
                byte p = (byte)r.Next(i);
                randoms[i] ^= randoms[p];
                randoms[p] ^= randoms[i];
                randoms[i] ^= randoms[p];
            }
            // disable all boxes
            for (byte i = 0; i < 9; ++i)
            {
                for (byte j = 0; j < 9; ++j)
                {
                    digitBoxes[i][j].Enabled = false;
                    digitBoxes[i][j].Text = "";
                }
            }
            // enable random boxes for player to fill in
            for (byte i = 0; i < NUMBLANK; ++i)
            {
                digitBoxes[randoms[i] / 9][randoms[i] % 9].Enabled = true;
            }
            for (byte i = 0; i < 9; ++i)
            {
                for (byte j = 0; j < 9; ++j)
                {
                    if (!digitBoxes[i][j].Enabled)
                    {
                        digitBoxes[i][j].Text = "" + board[i][j];
                    }
                }
            }
        }

        private const byte NUMBLANK = 47;
        private char[][] board;
    }
}
