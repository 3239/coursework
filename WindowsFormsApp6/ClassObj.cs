using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp6
{
    class Marka
    {
        public int Index { get; set; } = 0;
        public List<Marka> listSubMarks { get; set; } = new List<Marka>();
        public DataTable DTableMinus { get; set; }
        public DataTable DTable { get; set; }
        public DataTable DTablePlus { get; set; }
        public double[] ArrayM { get; set; }
        public double[] ArrayMM { get; set; }
        public double[] ArrayAlpha { get; set; }
        public double[] ArrayMProg { get; set; }
        public double[] ArrayAlphaProg { get; set; }

        public double A {get; set;}
        public double E { get; set; }


        public Marka(int index)
        {
            Index = index;
        }

        public Marka(int index, DataTable dTable, DataTable dTableMinus, DataTable dTablePlus, double a, double e)
        {
            Index = index;
            DTable = dTable;
            DTableMinus = dTableMinus;
            DTablePlus = dTablePlus;

            A = a;
            E = e;

        }

        public int GetCountRowTable()
        {
            return DTable.Rows.Count;
        }

        public void ChangeValuesTable()
        {
            for (int x = 0; x < GetCountRowTable(); x++)
            {
                for (int y = 1; y < DTable.Columns.Count; y++)
                {
                    DTablePlus.Rows[x][y] = Convert.ToDouble(DTable.Rows[x][y]) + E;
                    DTableMinus.Rows[x][y] = Convert.ToDouble(DTable.Rows[x][y]) - E;
                }
            }

        }

        public double AverageValue(double[] array)
        {
            double summ = 0;
            for (int x = 0; x < array.Length; x++)
                summ += array[x];
            return summ / array.Length;
        }

        public void CalkMProg()
        {
            ArrayMProg = new double[ArrayM.Length + 1];
            

            double[] tempArray = new double[ArrayM.Length];

            tempArray[0] = (A * ArrayM[0]) + ((1.0 - A) * AverageValue(ArrayM));

            for (int x = 1; x < ArrayM.Length; x++)
            {
                tempArray[x] = (A * ArrayM[x]) + ((1.0 - A) * tempArray[x - 1]);
            }

            for (int x = 0; x < tempArray.Length; x++)
            {
                ArrayMProg[x] = tempArray[x];
            }

            ArrayMProg[ArrayMProg.Length - 1] = (A * AverageValue(tempArray) + ((1 - A) * tempArray[tempArray.Length - 1]));

        }

        public void CalkAlphaProg()
        {
            ArrayAlphaProg = new double[ArrayAlpha.Length + 1];
           
            double[] tempArray = new double[ArrayAlpha.Length];

            tempArray[0] = (A * ArrayAlpha[0]) + ((1.0 - A) * AverageValue(ArrayAlpha));

            for (int x = 1; x < ArrayAlpha.Length; x++)
            {
                tempArray[x] = (A * ArrayAlpha[x]) + ((1.0 - A) * tempArray[x - 1]);
            }

            for (int x = 0; x < tempArray.Length; x++)
            {
                ArrayAlphaProg[x] = tempArray[x];
            }

            ArrayAlphaProg[ArrayAlphaProg.Length - 1] = (A * AverageValue(tempArray) + ((1 - A) * tempArray[tempArray.Length - 1]));
        }


        public void CalkM(DataTable dt)
        {
            ArrayM = new double[dt.Rows.Count];
            ArrayMM = new double[dt.Rows.Count];
            for (int row = 0; row < dt.Rows.Count; row++)
            {
                double summ = 0;
                for (int col = 1; col < dt.Columns.Count; col++)
                {
                    double value = Convert.ToDouble(dt.Rows[row][col]);
                    summ += Math.Pow(value, 2);
                }
                ArrayM[row] = Math.Sqrt(summ);
            }

            for (int row = 0; row < dt.Rows.Count; row++)
            {
                ArrayMM[row] = ArrayM[0] * ArrayM[row];
            }

        }

        public void CalkAlpha(DataTable dt)
        {
            ArrayAlpha = new double[dt.Rows.Count];
            double[] arrayRow2 = new double[dt.Rows.Count];

            for (int row = 0; row < dt.Rows.Count; row++)
            {
                double res = 0;
                for (int col = 1; col < dt.Columns.Count; col++)
                {
                    double value = Convert.ToDouble(dt.Rows[0][col]) * Convert.ToDouble(dt.Rows[row][col]);
                    res += value;
                }
                arrayRow2[row] = res;
            }


            for (int x = 0; x < ArrayMM.Length; x++)
            {
                if (arrayRow2[x] / ArrayMM[x] <= 1.0)
                    ArrayAlpha[x] = (Math.Acos(arrayRow2[x] / ArrayMM[x]) * 20626);
                else
                    ArrayAlpha[x] = 0;
            }
        }

        public double[] GetArrayEpoh()
        {
            double[] arrayEpoh = new double[DTable.Rows.Count];
            for (int row = 0; row < DTable.Rows.Count; row++)
            {
             arrayEpoh[row] =  Convert.ToInt32(DTable.Rows[row][0]);
            }
            return arrayEpoh;
        }


    }

    class ItemNameValue
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ItemNameValue(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
