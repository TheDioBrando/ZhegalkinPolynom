using System.Linq;
using System.IO;

namespace Zhegalkin
{
    public enum Variables
    {
        Constant = 0,
        x = 1,
        y = 2,
        z = 3,
        t = 4
    }
    // элементы массива принимают значения {0, 1}.
    public class Element
    {
        public Element()
        {
            Conjuction = new int[conjuctionLength];
            Next = null;
        }

        public Element(int[] array)
        {
            Conjuction = new int[conjuctionLength];
            for (var i = 0; i < 5; i++)
                Conjuction[i] = array[i];
        }

        public static int conjuctionLength = 5;
        public Element Next { get; set; }
        public int[] Conjuction { get; private set; }

        public int[] NullifyVar(int index)
        {
            var newConj = new int[conjuctionLength];
            for (var i = 0; i < conjuctionLength; i++)
                newConj[i] = Conjuction[i];
            newConj[index] = 0;
            return newConj;
        }

        public int Length
        {
            get
            {
                var count = 0;
                foreach (var item in Conjuction)
                    if (item == 1)
                        count++;
                return count;
            }
        }

        public static bool Equal(int[] conj1, int[] conj2)
        {
            for (var i = 0; i < 5; i++)
            {
                if (conj1[i] != conj2[i])
                    return false;
            }
            return true;
        }

    }

    public class Polynom
    {
        public Polynom()
        {
            Head = null;
            Tail = null;
        }

        public Polynom(Polynom copy)
        {
            Head = null;
            Tail = null;
            var tempForViewing = copy.Head;

            while (tempForViewing != null)
            {
                this.AddConj(tempForViewing.Conjuction.ToArray());
                tempForViewing = tempForViewing.Next;
            }
        }

        public Element Head { get; private set; }
        public Element Tail { get; private set; }

        public int ElementsCount
        {
            get
            {
                var count = 0;
                var temp = Head;
                while(temp != null)
                {
                    count++;
                    temp = temp.Next;
                }
                return count;
            }
        }

        // В качестве параметра получаем файл, данными которого заполняем список.
        // В текстовом файле каждая строка обозначает конъюнкцию элементов.
        // 1м элементом записывается константа 1, 2 элеменет обозначает х, 3 - у, 4 - z, 5 - t,
        
        public void CodeList(StreamReader file)
        {
            while (true)
            {
                var conjuction = file.ReadLine();
                if (conjuction == null || conjuction == "")
                    return;
                var elements = new int[5];
                elements[0] = 1;
                for(var i = 0; i < conjuction.Length; i++)
                {
                    switch(conjuction[i])
                    {
                        case 'x':
                            elements[1] = 1;
                            break;
                        case 'y':
                            elements[2] = 1;
                            break;
                        case 'z':
                            elements[3] = 1;
                            break;
                        case 't':
                            elements[4] = 1;
                            break;
                        default:
                            break;
                    }
                }
                AddConj(elements);
            }
        }

        // В текстовом файле каждая строка обозначает конъюнкцию элементов.
        public void DecodeList(StreamWriter file)
        {
            Element temp = Head;
            if (temp == null)
                return;
            while(temp != null)
            {
                if (temp.Length == 1)
                    file.WriteLine("1");
                else
                {
                    for (var i = 1; i < 5; i++)
                    {
                        if (temp.Conjuction[i] == 1)
                            file.Write((Variables)i);
                    }
                    file.WriteLine();
                }
                temp = temp.Next;
            }
            file.WriteLine("==================");
            Head = null;
            Tail = null;
        }

        public Element FindPrevConj( int[] conjuction)
        {
            Element temp = Head;
            if (temp == null || Element.Equal(temp.Conjuction, conjuction))
                return temp;
            while(temp.Next != null)
            {
                if (Element.Equal(temp.Next.Conjuction, conjuction))
                    return temp;
                temp = temp.Next;
            }
            return temp;
        }

        public bool AddConj(int[] conjuction)
        {
            if(Head == null)
            {
                Head = new Element(conjuction);
                Tail = Head;
                return true;
            }

            Element temp = FindPrevConj(conjuction);
            if (temp == Head && Element.Equal(conjuction, Head.Conjuction))
            {
                Head = Head.Next;
                return false;
            }

            if (temp == null || temp.Next == null)
            {
                Tail.Next = new Element(conjuction);
                Tail = Tail.Next;
                return true;
            }

            temp.Next = temp.Next.Next;
            return false;
        }

        public bool Remove(int position)
        {
            Element temp = Head;
            for(var i = 1; i < position - 1; i++)
            {
                if (temp == null || temp.Next == null)
                    return false;
                temp = temp.Next;
            }
            temp.Next = temp.Next.Next;
            return true;
        }

        public Polynom SortDesc()
        {
            if (Head == null)
                return null;
            var sortedPol = new Polynom();
            Element temp;
            var currentMinLength = 5;

            while (currentMinLength > 0)
            {
                temp = Head;
                while(temp != null)
                {
                    if(temp.Length == currentMinLength)
                    {
                        sortedPol.AddConj(temp.Conjuction.ToArray());
                    }
                    temp = temp.Next;
                }
                currentMinLength--;
            }
            return sortedPol;
        }
        
        public Polynom InvertVar(int varIndex)
        {
            if (Head == null)
                return null;
            var polWithConvertedVar = new Polynom();
            var temp = Head;
            while(temp != null)
            {
                if (temp.Conjuction[varIndex] != 0)
                {
                    polWithConvertedVar.AddConj(temp.Conjuction.ToArray());
                    polWithConvertedVar.AddConj(temp.NullifyVar(varIndex));
                }
                else
                    polWithConvertedVar.AddConj(temp.Conjuction.ToArray());
                temp = temp.Next;
            }
            return polWithConvertedVar;
        }

        public Polynom BuildPolynomConsistsOfOnlyThreeVars()
        {
            if (Head == null)
                return null;
            var rebuildPol = new Polynom();
            var temp = Head;

            while(temp != null)
            {
                if(temp.Length == 4)
                {
                    rebuildPol.AddConj(temp.Conjuction.ToArray());
                }
                temp = temp.Next;
            }
            return rebuildPol;
        }

        public static Polynom SumModule2(Polynom pol1, Polynom pol2)
        {
            if (pol1.Head == null && pol2.Head == null)
                return new Polynom();
            Polynom sumPol = null;
            if(pol1.Head != null)
                sumPol = new Polynom(pol1);
            if(pol2.Head != null)
            {
                var temp = pol2.Head;
                while(temp != null)
                {
                    sumPol.AddConj(temp.Conjuction.ToArray());
                    temp = temp.Next;
                }
            }
            return sumPol;
        }
    }

    class Program
    {
        static void Main()
        {
            var finTest3_1 = new StreamReader(@"C:\Users\tolya\source\repos\Zhegalkin\Test3(1).TXT");
            var finTest3_2 = new StreamReader(@"C:\Users\tolya\source\repos\Zhegalkin\Test3(2).TXT");
            var finTest1 = new StreamReader(@"C:\Users\tolya\source\repos\Zhegalkin\Test1.TXT");
            var fout = new StreamWriter(@"C:\Users\tolya\source\repos\Zhegalkin\Result.TXT",false);
            //Polynom pol = new Polynom();
            //pol.CodeList(finTest1);
            //var sortedPol = pol.SortDesc();
            //sortedPol.DecodeList(fout);
            //var threeVarPol = pol.BuildPolynomConsistsOfOnlyThreeVars();
            //threeVarPol.DecodeList(fout);
            //var invertedPol = pol.InvertVar(1);
            //invertedPol.DecodeList(fout);

            Polynom firstPol = new Polynom();
            firstPol.CodeList(finTest3_1);
            Polynom secondPol = new Polynom();
            secondPol.CodeList(finTest3_2);
            var sumOfTwoPol = Polynom.SumModule2(firstPol, secondPol);
            sumOfTwoPol.DecodeList(fout);
            finTest3_1.Close();
            finTest3_2.Close();
            finTest1.Close();
            fout.Close();
        }
    }
}
