using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;

namespace MBLNS.Models
{
    public class PVTSaturatedOilItem
    {
        public double Pb;
        public double Rs;
        public double Vo; // Viscosity 
        public double Bo;
    }

    public class PVTUndersaturatedOilItem
    {
        public PVTSaturatedOilItem BubblePoint;
        public List<double> Po = new List<double>();
        public List<double> Bo = new List<double>();
        public List<double> Vo = new List<double>();
    }

    class OilPVT
    {
        public Collection<PVTSaturatedOilItem>
            PVTSaturatedOilTable = new Collection<PVTSaturatedOilItem>();
        
        public Collection<PVTUndersaturatedOilItem>
            PVTUndersaturatedOilTable = new Collection<PVTUndersaturatedOilItem>();

        public void ReadFromFile(string filename)
        {
            PVTSaturatedOilTable.Clear();

            TextReader tr = new StreamReader(filename);

            string line;
            string[] lineSplit;
            char[] emptySeparator = new char[] { };

            bool PVTO = false;

            while ((line = tr.ReadLine()) != null)
            {
                if (line.Length == 0) continue;
                lineSplit = line.Split(emptySeparator, StringSplitOptions.RemoveEmptyEntries);

                if (lineSplit[0] == "PVTO") 
                {
                    PVTO = true;
                    continue;
                }

                if (lineSplit[0] == "/")
                {
                    PVTO = false;
                    continue;
                }
                
                // Строка данных PVTO, которая состоит из четырех значений и завершается разделителем
                // [ Rs   Pbub    Bo  ViscOil  / ] описывает одно состояние нефти при давлении насыщения и не имеет
                // данных по недонасыщенному состоянию, при P > Pbub. 

                if (PVTO && lineSplit.Length == 5)
                {
                    if (lineSplit[4] == "/")
                    {
                        PVTSaturatedOilTable.Add(
                            new PVTSaturatedOilItem
                            {
                                Rs = Convert.ToDouble(lineSplit[0]),
                                Pb = Convert.ToDouble(lineSplit[1]),
                                Bo = Convert.ToDouble(lineSplit[2]),
                                Vo = Convert.ToDouble(lineSplit[3])
                            });
                    }
                }

                // Строка данных PVTO, которая состоит из четырех значений, но не завершается разделителем
                // [ Rs   Pbub    Bo  ViscOil  ] описывает одно состояние нефти при давлении насыщения.
                // Далее ожидается набор параметров для недонасыщенной нефти

                if (PVTO && lineSplit.Length == 4)
                {
                    if (lineSplit[3] != "/")
                    {
                        PVTUndersaturatedOilItem Item = new PVTUndersaturatedOilItem();
                        
                        PVTSaturatedOilItem BubblePoint = new PVTSaturatedOilItem(){
                                Rs = Convert.ToDouble(lineSplit[0]),
                                Pb = Convert.ToDouble(lineSplit[1]),
                                Bo = Convert.ToDouble(lineSplit[2]),
                                Vo = Convert.ToDouble(lineSplit[3])};

                        PVTSaturatedOilTable.Add(BubblePoint);
                        Item.BubblePoint = BubblePoint;

                        bool PVTOextra = true;
                        string[] lineSplitExtra;
            
                        while (PVTOextra)
                        {
                            lineSplitExtra = tr.ReadLine().Split(emptySeparator, StringSplitOptions.RemoveEmptyEntries);

                            if (lineSplitExtra.Length == 3)
                            {
                                Item.Po.Add(Convert.ToDouble(lineSplitExtra[0]));
                                Item.Bo.Add(Convert.ToDouble(lineSplitExtra[1]));
                                Item.Vo.Add(Convert.ToDouble(lineSplitExtra[2]));
                                continue;
                            }

                            if ((lineSplitExtra.Length == 4) && (lineSplitExtra[3] == "/"))
                            {
                                Item.Po.Add(Convert.ToDouble(lineSplitExtra[0]));
                                Item.Bo.Add(Convert.ToDouble(lineSplitExtra[1]));
                                Item.Vo.Add(Convert.ToDouble(lineSplitExtra[2]));
                                PVTOextra = false;
                                continue;
                            }

                            PVTOextra = false; // Что-то пошло не так
                        }
                        PVTUndersaturatedOilTable.Add(Item);
                    }
                }
            }
        }
    }

}
