using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using System.Windows.Forms;

namespace FixMissingMSI
{
    public static class myFind
    {
        public static string lastFindText = "";
        
        public static DataGridView dataGridView1;
        public static int rowIdx = 0, cellIdx = 1;

        public static void IncreaseStep()
        {
            int rowCnt = dataGridView1.Rows.Count;
            int columnsCnt = dataGridView1.ColumnCount;
            cellIdx++;
            if (cellIdx >= columnsCnt)
            {
                rowIdx++;
                if (rowIdx >= rowCnt) rowIdx = 0;
                cellIdx = 1; //first column is the action column,so from 1 to search
            }


        }
        public static bool Find(string what)
        {
            if (lastFindText == "") return false;
            string what_lowercase = what.ToLower();
            int processCnt = 0;
            int totalCells = dataGridView1.Rows.Count * (dataGridView1.ColumnCount - 1);

            while (true)
            {
                try
                {
                    int rowCnt = dataGridView1.Rows.Count;
                    for (int r = rowIdx; r < rowCnt; r++)
                    {
                        DataGridViewRow row = dataGridView1.Rows[r];
                        int cnt = row.Cells.Count;
                        for (int i = cellIdx; i < cnt; i++)
                        {
                            processCnt++;

                            if (row.Cells[i].Value != null &&
                                row.Cells[i].Value.ToString().ToLower().Contains(what_lowercase))
                            {
                                rowIdx = r;
                                cellIdx = i;
                                dataGridView1.CurrentCell = row.Cells[i];
                             
                                return true;

                            }
                        }

                        //Need to set cellIdex=1, if the first row scan doesn't have match
                        cellIdx = 1;
                    }
                    //if we reach here, means it is not found to the end. need to 
                   
                    if (processCnt >= totalCells) break;
                    rowIdx = 0;
                    cellIdx = 1;

                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.ToString());
                }
            } //while true

            return false;
        }
    }

}
