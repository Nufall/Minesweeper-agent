using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace AIminesweeper
{
    class gridbox
    {
        public int X, Y;
        public int size = 30;
        public int col, row;
        public bool ismine = false;
        public bool isflagged = false;
        public bool isvisited = false;
        public int N_surrounding_mines = 0;

    }
    class grid
    {
        public List<gridbox> boxes = new List<gridbox>();
        public int Nrevealed = 0;
        public int Nbombs;
        public int gridsize;
        public int realvalue = 0;

    }

    class state
    {
        public grid gridstate = new grid();
        public int eval;
        public state(grid a)
        {
            gridbox pnn2;
            for (int j = 0; j < a.boxes.Count; j++)
            {
                pnn2 = new gridbox();
                pnn2.col = a.boxes[j].col;
                pnn2.row = a.boxes[j].row;
                pnn2.X = a.boxes[j].X;
                pnn2.Y = a.boxes[j].X;
                pnn2.size = a.boxes[j].size;
                pnn2.ismine = a.boxes[j].ismine;
                pnn2.isflagged = a.boxes[j].isflagged;
                pnn2.isvisited = a.boxes[j].isvisited;
                pnn2.N_surrounding_mines = a.boxes[j].N_surrounding_mines;
                this.gridstate.boxes.Add(pnn2);
            }
            this.gridstate.Nrevealed = a.Nrevealed;
            this.gridstate.gridsize = a.gridsize;
            this.gridstate.Nbombs = a.Nbombs;
            this.gridstate.realvalue = a.realvalue;
        }
    }
    class Program : Form
    {
        Bitmap off;
        grid grids = new grid();
        bool win = false;
        int isStart = 0;
        int ct = 0;
        //number of bombs and Gridsize
        int GS = 4, NB = 3;
        int maxval = 0;
        int ctrev = 0;
        int ctmarks = 0;
        int flagger = 0;
        int turn = 0;
        int min = 99999;
        int bombsavoided = 0;
        int gridsflagger=0;
        List<state> top3 = new List<state>();
        Program()
        {

            this.WindowState = FormWindowState.Maximized;
            this.Paint += Program_Paint;
            this.Load += Program_Load;
            this.MouseDown += Program_MouseDown;
            this.KeyDown += Program_KeyDown;
        }

        private void Program_KeyDown(object sender, KeyEventArgs e)
        {
            if (isStart == 0)
            {
                if (e.KeyCode == Keys.Up)
                {
                    GS++;
                }
                if (e.KeyCode == Keys.Down && GS > 4)
                {
                    GS--;
                }
                if (e.KeyCode == Keys.Right)
                {
                    if (NB < 0.15 * (GS * GS))
                    {
                        NB++;
                    }

                }
                if (e.KeyCode == Keys.Left && NB > 3)
                {
                    NB--;
                }
                if (e.KeyCode == Keys.Enter)
                {
                    setstage();
                }
            }
            if (isStart >= 2 && e.KeyCode == Keys.N)
            {
                agent();
            }
            DrawDubb(CreateGraphics());
        }

        private void Program_MouseDown(object sender, MouseEventArgs e)
        {

            if (isStart == 1)
            {
                for (int i = 0; i < grids.boxes.Count; i++)
                {
                    if (ct < grids.Nbombs)
                    {
                        if (e.X > grids.boxes[i].X && e.X < grids.boxes[i].X + grids.boxes[i].size)
                        {
                            if (e.Y > grids.boxes[i].Y && e.Y < grids.boxes[i].Y + grids.boxes[i].size)
                            {
                                grids.boxes[i].ismine = true;
                                ct++;
                            }
                        }
                    }
                }

                if (ct >= grids.Nbombs)
                {
                    isStart = 2;
                    surroundingminechecker();
                }
            }

            DrawDubb(CreateGraphics());
        }

        private void Program_Load(object sender, EventArgs e)
        {
            off = new Bitmap(ClientSize.Width, ClientSize.Height);
            isStart = 0;
            DrawDubb(CreateGraphics());
        }

        private void Program_Paint(object sender, PaintEventArgs e)
        {
            DrawDubb(e.Graphics);
        }

        void DrawDubb(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);
        }
        void DrawScene(Graphics g)
        {
            g.Clear(Color.LightBlue);
            Font drawFont = new System.Drawing.Font("Arial", 16);
            StringFormat drawFormat = new System.Drawing.StringFormat();
            int Margin = this.ClientSize.Width / 2;
            int Margin2 = this.ClientSize.Height / 2;
            int Margin3 = this.ClientSize.Width / 4;
            if (isStart >= 1)
            {
                for (int i = 0; i < (grids.gridsize * grids.gridsize); i++)
                {
                    if (grids.boxes[i].ismine == true && grids.boxes[i].isvisited == false)
                    {
                        g.FillRectangle(Brushes.Orange, Margin + grids.boxes[i].X, grids.boxes[i].Y, grids.boxes[i].size, grids.boxes[i].size);
                        g.DrawRectangle(Pens.White, Margin + grids.boxes[i].X, grids.boxes[i].Y, grids.boxes[i].size, grids.boxes[i].size);
                    }
                    if (grids.boxes[i].ismine == true && grids.boxes[i].isvisited == true)
                    {
                        g.FillRectangle(Brushes.Red, Margin + grids.boxes[i].X, grids.boxes[i].Y, grids.boxes[i].size, grids.boxes[i].size);
                        g.DrawRectangle(Pens.Black, Margin + grids.boxes[i].X, grids.boxes[i].Y, grids.boxes[i].size, grids.boxes[i].size);
                        g.DrawString("M", drawFont, Brushes.Black, grids.boxes[i].X, grids.boxes[i].Y, drawFormat);
                    }
                    if (grids.boxes[i].isvisited == true && grids.boxes[i].ismine == false)
                    {
                        g.FillRectangle(Brushes.White, Margin + grids.boxes[i].X, grids.boxes[i].Y, grids.boxes[i].size, grids.boxes[i].size);
                        g.DrawRectangle(Pens.Black, Margin + grids.boxes[i].X, grids.boxes[i].Y, grids.boxes[i].size, grids.boxes[i].size);
                        g.DrawString(grids.boxes[i].N_surrounding_mines.ToString(), drawFont, Brushes.Black, Margin + grids.boxes[i].X, grids.boxes[i].Y, drawFormat);
                    }
                    if (grids.boxes[i].isvisited == false && grids.boxes[i].ismine == false)
                    {
                        g.FillRectangle(Brushes.Gray, Margin + grids.boxes[i].X, grids.boxes[i].Y, grids.boxes[i].size, grids.boxes[i].size);
                        g.DrawRectangle(Pens.White, Margin + grids.boxes[i].X, grids.boxes[i].Y, grids.boxes[i].size, grids.boxes[i].size);
                    }
                    if (grids.boxes[i].isflagged == true)
                    {
                        g.FillRectangle(Brushes.Green, Margin + grids.boxes[i].X, grids.boxes[i].Y, grids.boxes[i].size, grids.boxes[i].size);
                        g.DrawRectangle(Pens.White, Margin + grids.boxes[i].X, grids.boxes[i].Y, grids.boxes[i].size, grids.boxes[i].size);
                    }

                    //the true solution
                    if (grids.boxes[i].ismine == true)
                    {
                        g.FillRectangle(Brushes.Red, grids.boxes[i].X, grids.boxes[i].Y, grids.boxes[i].size, grids.boxes[i].size);
                        g.DrawRectangle(Pens.Black, grids.boxes[i].X, grids.boxes[i].Y, grids.boxes[i].size, grids.boxes[i].size);
                        g.DrawString("M", drawFont, Brushes.Black, grids.boxes[i].X, grids.boxes[i].Y, drawFormat);
                    }
                    if (grids.boxes[i].ismine == false)
                    {
                        g.FillRectangle(Brushes.White, grids.boxes[i].X, grids.boxes[i].Y, grids.boxes[i].size, grids.boxes[i].size);
                        g.DrawRectangle(Pens.Black, grids.boxes[i].X, grids.boxes[i].Y, grids.boxes[i].size, grids.boxes[i].size);
                        g.DrawString(grids.boxes[i].N_surrounding_mines.ToString(), drawFont, Brushes.Black, grids.boxes[i].X, grids.boxes[i].Y, drawFormat);
                    }
                }
            }
            else
            {
                string s;
                //number of grids
                s = "decide grid size (this will be to the power of 2)";
                g.DrawString(s, drawFont, Brushes.Black, this.ClientSize.Width / 4, (this.ClientSize.Height / 4) - 30, drawFormat);
                g.FillRectangle(Brushes.White, this.ClientSize.Width / 4, this.ClientSize.Height / 4, 40, 40);
                g.DrawRectangle(Pens.Black, this.ClientSize.Width / 4, this.ClientSize.Height / 4, 40, 40);
                g.DrawString(GS.ToString(), drawFont, Brushes.Black, this.ClientSize.Width / 4, this.ClientSize.Height / 4, drawFormat);
                //number of bombs
                s = "decide number of bombs";
                g.DrawString(s, drawFont, Brushes.Black, this.ClientSize.Width / 4, (this.ClientSize.Height / 2) - 30, drawFormat);
                g.FillRectangle(Brushes.White, this.ClientSize.Width / 4, this.ClientSize.Height / 2, 40, 40);
                g.DrawRectangle(Pens.Black, this.ClientSize.Width / 4, this.ClientSize.Height / 2, 40, 40);
                g.DrawString(NB.ToString(), drawFont, Brushes.Black, this.ClientSize.Width / 4, this.ClientSize.Height / 2, drawFormat);
            }

            for (int m = 0; m < top3.Count; m++)
            {
                for (int m2 = 0; m2 < top3[m].gridstate.boxes.Count; m2++)
                {

                    if (top3[m].gridstate.boxes[m2].ismine == true && top3[m].gridstate.boxes[m2].isvisited == false)
                    {
                        g.FillRectangle(Brushes.Red, (Margin3 * m) + grids.boxes[m2].X, Margin2 + grids.boxes[m2].Y, grids.boxes[m2].size, grids.boxes[m].size);
                        g.DrawRectangle(Pens.White, (Margin3 * m) + grids.boxes[m2].X, Margin2 + grids.boxes[m2].Y, grids.boxes[m2].size, grids.boxes[m2].size);
                    }
                    if (top3[m].gridstate.boxes[m2].ismine == true && top3[m].gridstate.boxes[m2].isvisited == true)
                    {
                        g.FillRectangle(Brushes.Red, (Margin3 * m) + grids.boxes[m2].X, Margin2 + grids.boxes[m2].Y, grids.boxes[m2].size, grids.boxes[m2].size);
                        g.DrawRectangle(Pens.Black, (Margin3 * m) + grids.boxes[m2].X, Margin2 + grids.boxes[m2].Y, grids.boxes[m2].size, grids.boxes[m2].size);
                        g.DrawString("M", drawFont, Brushes.Black, (Margin3 * m) + grids.boxes[m2].X, Margin2 + grids.boxes[m2].Y, drawFormat);
                    }
                    if (top3[m].gridstate.boxes[m2].isvisited == true && top3[m].gridstate.boxes[m2].ismine == false)
                    {
                        g.FillRectangle(Brushes.White, (Margin3 * m) + grids.boxes[m2].X, Margin2 + grids.boxes[m2].Y, grids.boxes[m2].size, grids.boxes[m2].size);
                        g.DrawRectangle(Pens.Black, (Margin3 * m) + grids.boxes[m2].X, Margin2 + grids.boxes[m2].Y, grids.boxes[m2].size, grids.boxes[m2].size);
                        g.DrawString(grids.boxes[m2].N_surrounding_mines.ToString(), drawFont, Brushes.Black, (Margin3 * m) + grids.boxes[m2].X, Margin2 + grids.boxes[m2].Y, drawFormat);
                    }
                    if (top3[m].gridstate.boxes[m2].isvisited == false && top3[m].gridstate.boxes[m2].ismine == false)
                    {
                        g.FillRectangle(Brushes.Gray, (Margin3 * m) + grids.boxes[m2].X, Margin2 + grids.boxes[m2].Y, grids.boxes[m2].size, grids.boxes[m2].size);
                        g.DrawRectangle(Pens.White, (Margin3 * m) + grids.boxes[m2].X, Margin2 + grids.boxes[m2].Y, grids.boxes[m2].size, grids.boxes[m2].size);
                    }
                    if (top3[m].gridstate.boxes[m2].isflagged == true)
                    {
                        g.FillRectangle(Brushes.Green, (Margin3 * m) + grids.boxes[m2].X, Margin2 + grids.boxes[m2].Y, grids.boxes[m2].size, grids.boxes[m2].size);
                        g.DrawRectangle(Pens.White, (Margin3 * m) + grids.boxes[m2].X, Margin2 + grids.boxes[m2].Y, grids.boxes[m2].size, grids.boxes[m2].size);
                    }
                }
                //generate randomstates as it gets solved

            }
            top3.Clear();
        }
        //------------------------------------------------------------
        void setstage()
        {
            //take in gridsize (GS*GS is box number) and mine number (Nbombs)
            grids.gridsize = GS;
            grids.Nbombs = NB;
            gridbox pnn;
            int posx = 0;
            int posy = 0;
            int col = 0;
            int row = 0;
            for (int i = 0; i < grids.gridsize; i++)
            {
                for (int k = 0; k < grids.gridsize; k++)
                {
                    pnn = new gridbox();
                    pnn.X = 15 + posx;
                    pnn.Y = 15 + posy;
                    pnn.col = col;
                    pnn.row = row;

                    col++;
                    posx += pnn.size;
                    grids.boxes.Add(pnn);
                }

                row++;
                posx = 0;
                col = 0;
                posy += 30;
            }
            //create list of grids
            isStart = 1;
        }
        void surroundingminechecker()
        {
            //checking if surrounding boxes are a mine or not
            int loop = grids.gridsize * grids.gridsize;
            for (int i = 0; i < loop; i++)
            {
                //check if bomb is around
                int row = grids.boxes[i].row;
                int col = grids.boxes[i].col;
                for (int j = 0; j < loop; j++)
                {


                    if (grids.boxes[j].row == row && grids.boxes[j].col == (col - 1))
                    {

                        if (grids.boxes[j].ismine == true)
                        {
                            grids.boxes[i].N_surrounding_mines++;
                        }

                    }
                    if (grids.boxes[j].row == row && grids.boxes[j].col == (col + 1))
                    {
                        if (grids.boxes[j].ismine == true)
                        {
                            grids.boxes[i].N_surrounding_mines++;
                        }
                    }
                    if (grids.boxes[j].row == (row + 1) && grids.boxes[j].col == col)
                    {
                        if (grids.boxes[j].ismine == true)
                        {
                            grids.boxes[i].N_surrounding_mines++;
                        }
                    }
                    if (grids.boxes[j].row == (row - 1) && grids.boxes[j].col == col)
                    {
                        if (grids.boxes[j].ismine == true)
                        {
                            grids.boxes[i].N_surrounding_mines++;
                        }
                    }
                    if (grids.boxes[j].row == (row + 1) && grids.boxes[j].col == (col + 1))
                    {
                        if (grids.boxes[j].ismine == true)
                        {
                            grids.boxes[i].N_surrounding_mines++;
                        }
                    }
                    if (grids.boxes[j].row == (row - 1) && grids.boxes[j].col == (col + 1))
                    {
                        if (grids.boxes[j].ismine == true)
                        {
                            grids.boxes[i].N_surrounding_mines++;
                        }
                    }
                    if (grids.boxes[j].row == (row - 1) && grids.boxes[j].col == (col - 1))
                    {
                        if (grids.boxes[j].ismine == true)
                        {
                            grids.boxes[i].N_surrounding_mines++;
                        }
                    }
                    if (grids.boxes[j].row == (row + 1) && grids.boxes[j].col == (col - 1))
                    {
                        if (grids.boxes[j].ismine == true)
                        {
                            grids.boxes[i].N_surrounding_mines++;
                        }
                    }

                }

            }
        }
        int revealboxes(int M, grid Sgrid)
        {
            ctmarks = 0;
                
                if (Sgrid.boxes[M].isvisited == false)
                {
                    if (Sgrid.boxes[M].ismine == true)
                    {
                    return -999;     
                    }
                    Sgrid.Nrevealed++;               
                    Sgrid.boxes[M].isvisited = true;  
                    if (Sgrid.boxes[M].N_surrounding_mines == 0)
                    {
                        //directly above
                        if (Sgrid.boxes[M].row > 0)
                        {
                            revealboxes(M - Sgrid.gridsize, Sgrid);
                        }
                        //directly below
                        if (Sgrid.boxes[M].row < Sgrid.gridsize - 1)
                        {
                            revealboxes(M + Sgrid.gridsize, Sgrid);
                        }
                        //left of it
                        if (Sgrid.boxes[M].col > 0)
                        {
                            revealboxes(M - 1, Sgrid);
                        }
                        //right of it
                        if (Sgrid.boxes[M].col < Sgrid.gridsize - 1)
                        {
                            revealboxes(M + 1, Sgrid);
                        }
                        //left up corner
                        if (Sgrid.boxes[M].row > 0 && Sgrid.boxes[M].col > 0)
                        {
                            revealboxes((M - grids.gridsize) - 1, Sgrid);
                        }
                        //right up corner
                        if (Sgrid.boxes[M].row > 0 && Sgrid.boxes[M].col < Sgrid.gridsize - 1)
                        {
                            revealboxes((M - Sgrid.gridsize) + 1, Sgrid);
                        }
                        //left down corner
                        if (Sgrid.boxes[M].row < Sgrid.gridsize - 1 && Sgrid.boxes[M].col > 0)
                        {
                            revealboxes((M + Sgrid.gridsize) - 1, Sgrid);
                        }
                        //right down corner
                        if (Sgrid.boxes[M].row < Sgrid.gridsize - 1 && Sgrid.boxes[M].col < Sgrid.gridsize - 1)
                        {
                            revealboxes((M + Sgrid.gridsize) + 1, Sgrid);
                        }

                }
                Sgrid.realvalue = Sgrid.Nrevealed;
                return Sgrid.realvalue;
            }

                

            if (Sgrid.boxes[M].isvisited == true)
            {
                //directly above
                if (Sgrid.boxes[M].row > 0)
                {
                    if (Sgrid.boxes[M - Sgrid.gridsize].isflagged)
                    {
                        ctmarks++;
                    }
                }
                //directly below
                if (Sgrid.boxes[M].row < Sgrid.gridsize - 1)
                {
                    if (Sgrid.boxes[M + Sgrid.gridsize].isflagged)
                    {
                        ctmarks++;
                    }
                }
                //left of it
                if (Sgrid.boxes[M].col > 0)
                {
                    if (Sgrid.boxes[M - 1].isflagged)
                    {
                        ctmarks++;
                    }
                }
                //right of it
                if (Sgrid.boxes[M].col < Sgrid.gridsize - 1)
                {
                    if (Sgrid.boxes[M + 1].isflagged)
                    {
                        ctmarks++;
                    }
                }
                //left up corner
                if (Sgrid.boxes[M].row > 0 && Sgrid.boxes[M].col > 0)
                {
                    if (Sgrid.boxes[(M - grids.gridsize) - 1].isflagged)
                    {
                        ctmarks++;
                    }
                }
                //right up corner
                if (Sgrid.boxes[M].row > 0 && Sgrid.boxes[M].col < Sgrid.gridsize - 1)
                {
                    if (Sgrid.boxes[(M - Sgrid.gridsize) + 1].isflagged)
                    {
                        ctmarks++;
                    }
                }
                //left down corner
                if (Sgrid.boxes[M].row < Sgrid.gridsize - 1 && Sgrid.boxes[M].col > 0)
                {
                    if (Sgrid.boxes[(M + Sgrid.gridsize) - 1].isflagged)
                    {
                        ctmarks++;
                    }
                }
                //right down corner
                if (Sgrid.boxes[M].row < Sgrid.gridsize - 1 && Sgrid.boxes[M].col < Sgrid.gridsize - 1)
                {
                    if (Sgrid.boxes[(M + Sgrid.gridsize) + 1].isflagged)
                    {
                        ctmarks++;
                    }
                }
                if (ctmarks == Sgrid.boxes[M].N_surrounding_mines)
                {
                    if (Sgrid.boxes[M].row > 0)
                    {
                        if (Sgrid.boxes[M - Sgrid.gridsize].isvisited == false)
                        {
                            revealboxes(M - Sgrid.gridsize, Sgrid);
                        }
                    }
                    //directly below
                    if (Sgrid.boxes[M].row < Sgrid.gridsize - 1)
                    {
                        if (Sgrid.boxes[M + Sgrid.gridsize].isvisited == false)
                        {
                            revealboxes(M + Sgrid.gridsize, Sgrid);
                        }
                    }
                    //left of it
                    if (Sgrid.boxes[M].col > 0)
                    {
                        if (Sgrid.boxes[M - 1].isvisited == false)
                        {
                            revealboxes(M - 1, Sgrid);
                        }
                    }
                    //right of it
                    if (Sgrid.boxes[M].col < Sgrid.gridsize - 1)
                    {
                        if (Sgrid.boxes[M + 1].isvisited == false)
                        {
                            revealboxes(M + 1, Sgrid);
                        }
                    }
                    //left up corner
                    if (Sgrid.boxes[M].row > 0 && Sgrid.boxes[M].col > 0)
                    {
                        if (Sgrid.boxes[(M - grids.gridsize) - 1].isvisited == false)
                        {
                            revealboxes((M - grids.gridsize) - 1, Sgrid);
                        }
                    }
                    //right up corner
                    if (Sgrid.boxes[M].row > 0 && Sgrid.boxes[M].col < Sgrid.gridsize - 1)
                    {
                        if (Sgrid.boxes[(M - Sgrid.gridsize) + 1].isvisited == false)
                        {
                            revealboxes((M - Sgrid.gridsize) + 1, Sgrid);
                        }
                    }
                    //left down corner
                    if (Sgrid.boxes[M].row < Sgrid.gridsize - 1 && Sgrid.boxes[M].col > 0)
                    {
                        if (Sgrid.boxes[(M + Sgrid.gridsize) - 1].isvisited == false)
                        {
                            revealboxes((M + Sgrid.gridsize) - 1, Sgrid);
                        }
                    }
                    //right down corner
                    if (Sgrid.boxes[M].row < Sgrid.gridsize - 1 && Sgrid.boxes[M].col < Sgrid.gridsize - 1)
                    {
                        if (Sgrid.boxes[(M + Sgrid.gridsize) + 1].isvisited == false)
                        {
                            revealboxes((M + Sgrid.gridsize) + 1, Sgrid);
                        }
                    }


                }
                
                Sgrid.realvalue = Sgrid.Nrevealed;
                return Sgrid.realvalue;

            }
            return 0;
        }

        int flagboxes(int M, grid Sgrid)
        {
            if (Sgrid.boxes[M].N_surrounding_mines != 0)
            {
                int ct = 0;//if revealed or flagged
                int ct2 = 0;//if flagged only
                List<int> hidden = new List<int>();
                int pnn;
                if (Sgrid.boxes[M].row > 0)
                {
                    if (Sgrid.boxes[M - Sgrid.gridsize].isvisited == true || Sgrid.boxes[M - Sgrid.gridsize].isflagged == true)
                    {
                        ct++;
                    }
                    if (Sgrid.boxes[M - Sgrid.gridsize].isflagged == true)
                    {
                        ct2++;
                    }
                    if (Sgrid.boxes[M - Sgrid.gridsize].isflagged == false && Sgrid.boxes[M - Sgrid.gridsize].ismine == true)
                    {
                        pnn = M - Sgrid.gridsize;
                        hidden.Add(pnn);
                    }
                }
                if (Sgrid.boxes[M].row < Sgrid.gridsize - 1)
                {
                    if (Sgrid.boxes[M + Sgrid.gridsize].isvisited == true || Sgrid.boxes[M + Sgrid.gridsize].isflagged == true)
                    {
                        ct++;
                    }
                    if (Sgrid.boxes[M + Sgrid.gridsize].isflagged == true)
                    {
                        ct2++;
                    }
                    if (Sgrid.boxes[M + Sgrid.gridsize].isflagged == false && Sgrid.boxes[M + Sgrid.gridsize].ismine == true)
                    {
                        pnn = M + Sgrid.gridsize;
                        hidden.Add(pnn);
                    }
                }
                if (Sgrid.boxes[M].col > 0)
                {
                    if (Sgrid.boxes[M - 1].isvisited == true || Sgrid.boxes[M - 1].isflagged == true)
                    {
                        ct++;
                    }
                    if (Sgrid.boxes[M - 1].isflagged == true)
                    {
                        ct2++;
                    }
                    if (Sgrid.boxes[M - 1].isflagged == false && Sgrid.boxes[M - 1].ismine == true)
                    {
                        pnn = M - 1;
                        hidden.Add(pnn);
                    }
                }
                if (Sgrid.boxes[M].col < Sgrid.gridsize - 1)
                {
                    if (Sgrid.boxes[M + 1].isvisited == true || Sgrid.boxes[M + 1].isflagged == true)
                    {
                        ct++;
                    }
                    if (Sgrid.boxes[M + 1].isflagged == true)
                    {
                        ct2++;
                    }
                    if (Sgrid.boxes[M + 1].isflagged == false && Sgrid.boxes[M + 1].ismine == true)
                    {
                        pnn = M + 1;
                        hidden.Add(pnn);
                    }
                }
                if (Sgrid.boxes[M].row > 0 && Sgrid.boxes[M].col > 0)
                {
                    if (Sgrid.boxes[(M - grids.gridsize) - 1].isvisited == true || Sgrid.boxes[(M - grids.gridsize) - 1].isflagged == true)
                    {
                        ct++;
                    }
                    if (Sgrid.boxes[(M - grids.gridsize) - 1].isflagged == true)
                    {
                        ct2++;
                    }
                    if (Sgrid.boxes[(M - grids.gridsize) - 1].isflagged == false && Sgrid.boxes[(M - grids.gridsize) - 1].ismine == true)
                    {
                        pnn = (M - grids.gridsize) - 1;
                        hidden.Add(pnn);
                    }
                }
                if (Sgrid.boxes[M].row > 0 && Sgrid.boxes[M].col < Sgrid.gridsize - 1)
                {
                    if (Sgrid.boxes[(M - Sgrid.gridsize) + 1].isvisited == true || Sgrid.boxes[(M - Sgrid.gridsize) + 1].isflagged == true)
                    {
                        ct++;
                    }
                    if (Sgrid.boxes[(M - Sgrid.gridsize) + 1].isflagged == true)
                    {
                        ct2++;
                    }
                    if (Sgrid.boxes[(M - Sgrid.gridsize) + 1].isflagged == false && Sgrid.boxes[(M - Sgrid.gridsize) + 1].ismine == true)
                    {
                        pnn = (M - Sgrid.gridsize) + 1;
                        hidden.Add(pnn);
                    }
                }
                if (Sgrid.boxes[M].row < Sgrid.gridsize - 1 && Sgrid.boxes[M].col > 0)
                {
                    if (Sgrid.boxes[(M + Sgrid.gridsize) - 1].isvisited == true || Sgrid.boxes[(M + Sgrid.gridsize) - 1].isflagged == true)
                    {
                        ct++;
                    }
                    if (Sgrid.boxes[(M + Sgrid.gridsize) - 1].isflagged == true)
                    {
                        ct2++;
                    }
                    if (Sgrid.boxes[(M + Sgrid.gridsize) - 1].isflagged == false && Sgrid.boxes[(M + Sgrid.gridsize) - 1].ismine == true)

                    {
                        pnn = (M + Sgrid.gridsize) - 1;
                        hidden.Add(pnn);
                    }
                }
                if (Sgrid.boxes[M].row < Sgrid.gridsize - 1 && Sgrid.boxes[M].col < Sgrid.gridsize - 1)
                {
                    if (Sgrid.boxes[(M + Sgrid.gridsize) + 1].isvisited == true || Sgrid.boxes[(M + Sgrid.gridsize) + 1].isflagged == true)
                    {
                        ct++;
                    }
                    if (Sgrid.boxes[(M + Sgrid.gridsize) + 1].isflagged == true)
                    {
                        ct2++;
                    }
                    if (Sgrid.boxes[(M + Sgrid.gridsize) + 1].isflagged == false && Sgrid.boxes[(M + Sgrid.gridsize) + 1].ismine == true)
                    {
                        pnn = (M + Sgrid.gridsize) + 1;
                        hidden.Add(pnn);
                    }
                }

                if (8 - ct == Sgrid.boxes[M].N_surrounding_mines - ct2)
                {
                    if (hidden.Count != 0)
                    {
                        for (int flags = 0; flags < hidden.Count; flags++)
                        {
                            Sgrid.boxes[hidden[flags]].isflagged = true;
                        }
                        Sgrid.realvalue += (hidden.Count*(-1)) + 5;
                        return Sgrid.realvalue;
                    }

                }

            }
            return 0;

        }
        void agent()
        {
            state statepnn;
            List<state> Lgrids = new List<state>();
            int max = -99999, pos = 0, k, j, i;
            Random rr = new Random();
            //check if the answer is already found
            if (win == true)
            { return; }
            //loop to look at possibilities(children)
            if(flagger==1)
            {
                for (k = 0; k < grids.boxes.Count; k++)
                {
                    //step 1: copystate and create a variant/child 

                    if (grids.boxes[k].isvisited == true && flagger == 1)
                    {
                        turn = 1;
                        statepnn = new state(grids);
                        statepnn.eval = flagboxes(k, statepnn.gridstate);
                        Lgrids.Add(statepnn);

                    }

                }
                flagger = 0;
            }

            if (flagger == 0)
            {
                for (k = 0; k < grids.boxes.Count; k++)
                {
                    //step 1: copystate and create a variant/child 
                    if (grids.boxes[k].isvisited == false && ctrev == 0 && flagger == 0)
                    {
                        turn = 0;
                        statepnn = new state(grids);
                        statepnn.eval = revealboxes(k, statepnn.gridstate);

                        Lgrids.Add(statepnn);

                    }

                    if (grids.boxes[k].isvisited == true && ctrev > 0 && flagger == 0)
                    {
                        turn = 0;
                        statepnn = new state(grids);
                        statepnn.eval = revealboxes(k, statepnn.gridstate);
                        Lgrids.Add(statepnn);

                    }
                }
                flagger = 1;
            }

           


            //step 2: choose child with the highest eval and choose highest 3 children to display
            if(turn==0)
            {
                int counter = 0;
                int gridscounter = 0;
                for (j = 0; j < Lgrids.Count; j++)
                {
                    if (max < Lgrids[j].eval)
                    {
                        max = Lgrids[j].eval;
                        pos = j;
                    }
                    if (counter < 3)
                    {
                        gridsflagger = 0;
                        int randomstate = rr.Next(0, Lgrids.Count - 1);
                        if (Lgrids[randomstate].eval != 0)
                        {
                            if (Lgrids[randomstate].eval != grids.realvalue)
                            {
                                statepnn = new state(Lgrids[randomstate].gridstate);
                                statepnn.eval = Lgrids[randomstate].eval;
                                top3.Add(statepnn);
                                counter++;
                            }


                        }

                    }
                }
                
            }
            if (turn == 1)
            {
                int counter = 0;
                for (j = 0; j < Lgrids.Count; j++)
                {
                    if (min > Lgrids[j].eval)
                    {
                        min = Lgrids[j].eval;
                        pos = j;
                    }
                    if (counter < 3)
                    {
                        gridsflagger = 0;
                        int randomstate = rr.Next(0, Lgrids.Count - 1);
                        if (Lgrids[randomstate].eval != 0)
                        {
                            if (Lgrids[randomstate].eval !=grids.realvalue)
                            {
                                statepnn = new state(Lgrids[randomstate].gridstate);
                                statepnn.eval = Lgrids[randomstate].eval;
                                top3.Add(statepnn);
                                counter++;
                            }


                        }

                    }
                }

            }




      
            //step3: set the state chosen as the current node to expand
            for (i = 0; i < grids.boxes.Count; i++)
            {
                grids.boxes[i].isvisited = Lgrids[pos].gridstate.boxes[i].isvisited;
                grids.boxes[i].isflagged = Lgrids[pos].gridstate.boxes[i].isflagged;
                ctrev++;
            }
            if (maxval == Lgrids[pos].eval)
            {
                ctrev = 0;
            }
            grids.Nrevealed = Lgrids[pos].gridstate.Nrevealed;

            grids.realvalue = Lgrids[pos].gridstate.realvalue;
            maxval = Lgrids[pos].eval;

        }


        static void Main()
        {
            Program obj;
            obj = new Program();
            Application.Run(obj);
        }
    }
}
