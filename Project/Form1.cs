using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            //InitializeComponent();
            this.Size = new Size(818, 497);
            this.Text = "Нахождение корней булевых уравнений";
            starting_controls();
        }
        TextBox[,] equations;
        bool[] type;
        int k = 3;
        int n = 3;
        int equation_number = 1;
        string value_equals = "1";
        string[] answers;
        string[] generate(int k, int n) //генератор уравнений
        {
            Random rand = new Random();
            string s = "";
            int k1 = k;
            int inv;
            int type;
            bool isKNF;
            type = rand.Next(0, 2); //1 - KNF, 0 - DNF
            if (type == 0)
            {
                isKNF = false;
            }
            else isKNF = true;
            for (int i = 0; i < k1; i++)
            {
                string Alphabet = "abcdefghijklmnopqrstuvwxyz";
                s += "(";
                int k2 = rand.Next(1, n + 1);
                for (int j = 0; j < k2; j++)
                {
                    inv = rand.Next(0, 2);
                    if (inv == 1)
                    {
                        s += "!";
                    }
                    int k3 = rand.Next(0, Alphabet.Length);
                    s += Alphabet[k3].ToString();
                    Alphabet = Alphabet.Remove(k3, 1);
                    if (isKNF)
                    {
                        s += "|";
                    }
                    else
                    {
                        s += "&";
                    }

                }
                s = s.Remove(s.Length - 1, 1);
                if (isKNF)
                {
                    s += ") & ";
                }
                else
                {
                    s += ") | ";
                }

            }
            s = s.Remove(s.Length - 3, 3);
            string[] result = new string[2];
            result[0] = s;
            if (isKNF)
                result[1] = "1";
            else result[1] = "0";
            return result;
        }
        string sub_algoritm(string[,] Vectors, bool isKNF, int count) //если КНФ=0 или ДНФ=1
        {
            //Для КНФ=0 хотя бы одна скобка равна 0, для ДНФ=1 хотя бы одна скобка равна 1
            string s = new string('-', Vectors[0, 0].Length);   //ответ в виде строки "----"
            StringBuilder result = new StringBuilder(s); //здесь будет ответ
            //делаем так, чтобы count скобка удовлетворяла условиям
            string m = Vectors[count, 0];
            string p = Vectors[count, 1];
            if (isKNF)
            {
                //count скобка равна 0 в КНФ, если каждая буква равна 0
                for (int j = 0; j < m.Length; j++) //цикл по компоненте в первой скобке
                {
                    if ((m[j].Equals('1')) && (p[j].Equals('1'))) //буква без отрицания равна 0
                    {
                        replace_vect(Vectors, j, 0, true);
                        result[j] = '0';//новый ответ
                    }
                    else if ((m[j].Equals('1')) && (p[j].Equals('0'))) //буква с отрицанием равна 1
                    {
                        replace_vect(Vectors, j, 1, true);
                        result[j] = '1';//новый ответ
                    }
                }
            }
            if (!isKNF)
            {
                //count скобка равна 1 в ДНФ, если каждая буква равна 1
                for (int j = 0; j < m.Length; j++) //цикл по компоненте в первой скобке
                {
                    if ((m[j].Equals('1')) && (p[j].Equals('1'))) //буква без отрицания равна 1
                    {
                        replace_vect(Vectors, j, 1, false);
                        result[j] = '1';//новый ответ
                    }
                    else if ((m[j].Equals('1')) && (p[j].Equals('0'))) //буква с отрицанием равна 0
                    {
                        replace_vect(Vectors, j, 0, false);
                        result[j] = '0';//новый ответ
                    }
                }
            }
            return result.ToString();
        }
        void greedy_algoritm(string[,] Vectors, bool isKNF, StringBuilder result) //жадный алгоритм, ищет следующую букву для замены и заменяет её
        {
            for (int i = 0; i < Vectors[0, 0].Length; i++) //цикл по компоненте
            {
                int max = 0;                                //максимальное число векторов с единицей на позиции maxi
                int maxi = 0;                               //какая буква под max
                int k0 = 0;
                int k1 = 0;
                for (int j = 0; j < Vectors.GetLength(0); j++) //цикл по векторам
                {
                    string m = Vectors[j, 0];
                    string p = Vectors[j, 1];
                    if (m[i].Equals('1')) //выбираем вектора, где m[i]=1
                    {                       //в этих векторах считаем
                        if (p[i].Equals('0'))   //p[i] = 0
                        {
                            k0++;
                        }
                        if (p[i].Equals('1')) //p[i] = 1
                        {
                            k1++;
                        }
                    }
                } //запоминаем максимум
                if (k0 > max)
                {
                    max = k0;
                    maxi = 0;
                }
                if (k1 >= max)
                {
                    max = k1;
                    maxi = 1;
                }
                //пусть p[i] = maxi, цикл по векторам:
                for (int j = 0; j < Vectors.GetLength(0); j++)
                {
                    string m = Vectors[j, 0];
                    string p = Vectors[j, 1];
                    if ((m[i].Equals('1')) && (p[i].Equals((char)maxi)) && (isKNF))
                    {
                        //соответствующая конъюнкция равна 1
                        if (maxi == 1)
                        {
                            replace_vect(Vectors, i, 1, true);
                            result[i] = '1';
                        }
                        else
                        {
                            replace_vect(Vectors, i, 0, true);
                            result[i] = '0';//новый ответ
                        }
                    }
                    if ((m[i].Equals('1')) && (p[i].Equals((char)maxi)) && !(isKNF))
                    {
                        //соответствующая дизъюнкция равна 0
                        if (maxi == 1)
                        {
                            replace_vect(Vectors, i, 0, false);
                            result[i] = '0';
                        }
                        else
                        {
                            replace_vect(Vectors, i, 1, false);
                            result[i] = '1';//новый ответ
                        }
                    }
                    if ((m[i].Equals('1')) && !(p[i].Equals((char)maxi)) && (isKNF))
                    {
                        //полагаем m[i] = 0 
                        //однако надо проверить, что нет такой ситуации: (a+!b)&(a+b)=1. В таком
                        //случае нельзя выбирать a = 0.
                        //для начала найдём векторы с m[i] = 1 и m[j] = 1, где i!=j. Остальные должны быть равны 0.
                        Boolean flag = true;
                        for (int k = 0; k < Vectors.GetLength(0); k++)
                        {
                            if (Vectors[k, 0].Count(f => f == '1') == 2) //если количество единиц равно 2
                            {
                                int q = -1;
                                for (int w = 0; w < Vectors[0, 0].Length; w++) //проходим по буквам этого вектора
                                {
                                    if ((Vectors[k, 0][w].Equals('1')) && (i != w)) //находим j != i
                                    {
                                        q = w;  //та самая j
                                        break;
                                    }
                                }
                                char value = '-';
                                //нашли i и j. Сравним p[i] и p[j]
                                for (int w = 0; w < Vectors.GetLength(0); w++)
                                {
                                    if (Vectors[w, 0].Count(f => f == '1') == 2) //если количество единиц равно 2
                                    {
                                        if (value == '-') //первый найденный вектор
                                        {
                                            value = Vectors[w, 1][q];
                                        }
                                        else
                                        {
                                            if ((Vectors[w, 0].Equals('1')) & (!Vectors[w, 1][q].Equals(value))) //два разных значения
                                            {
                                                flag = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (flag == false)
                        //получилась хотя бы одна ситуация, когда (a+!b)&(a+b)=1
                        //значит заменять а на 0 нельзя
                        //a = 1, значит такие ДНФ = 1. Нужно обнулять векторы
                        {
                            replace_vect(Vectors, i, maxi, true);
                            if (maxi == 1)
                            {
                                result[i] = '1'; //новый ответ
                            }
                            else
                            {
                                result[i] = '0'; //новый ответ
                            }
                        }
                        else //иначе можно обнулить вектор, a = (maxi)
                        {
                            if (maxi == 1) //чаще встречается буква без отрицания
                            {
                                replace_vect(Vectors, i, 1, true);
                                result[i] = '1'; //новый ответ
                            }
                            else //чаще встречается буква с отрицанием
                            {
                                replace_vect(Vectors, i, 0, true);
                                result[i] = '0'; //новый ответ
                            }
                        }

                    }
                    else if ((m[i].Equals('1')) && !(p[i].Equals((char)maxi)) && !(isKNF))
                    {
                        //полагаем m[i] = 0 
                        //однако надо проверить, что нет такой ситуации: (a&!b)+(a&b)=0. В таком
                        //случае нельзя выбирать a = 1.
                        //для начала найдём векторы с m[i] = 1 и m[j] = 1, где i!=j. Остальные должны быть равны 0.
                        Boolean flag = true;
                        for (int k = 0; k < Vectors.GetLength(0); k++)
                        {
                            if (Vectors[k, 0].Count(f => f == '1') == 2) //если количество единиц равно 2
                            {
                                int q = -1;
                                for (int w = 0; w < Vectors[0, 0].Length; w++) //проходим по буквам этого вектора
                                {
                                    if ((Vectors[k, 0][w].Equals('1')) && (i != w)) //находим j != i
                                    {
                                        q = w;  //та самая j
                                        break;
                                    }
                                }
                                char value = '-';
                                //нашли i и j. Сравним p[i] и p[j]
                                for (int w = 0; w < Vectors.GetLength(0); w++)
                                {
                                    if (Vectors[w, 0].Count(f => f == '1') == 2) //если количество единиц равно 2
                                    {
                                        if (value == '-') //первый найденный вектор
                                        {
                                            value = Vectors[w, 1][q];
                                        }
                                        else
                                        {
                                            if ((Vectors[w, 0].Equals('1')) & (!Vectors[w, 1][q].Equals(value))) //два разных значения
                                            {
                                                flag = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (flag == false)
                        //получилась хотя бы одна ситуация, когда (a&!b)+(a&b)=0
                        //значит заменять а на 1 нельзя
                        //a = 0, значит такие КНФ = 0. Нужно обнулять векторы
                        {
                            replace_vect(Vectors, i, maxi, false);
                            if (maxi == 1)
                            {
                                result[i] = '0'; //новый ответ
                            }
                            else
                            {
                                result[i] = '1'; //новый ответ
                            }
                        }
                        else //иначе можно обнулить вектор, a = (maxi)
                        {
                            if (maxi == 1) //чаще встречается буква без отрицания
                            {
                                replace_vect(Vectors, i, 0, false);
                                result[i] = '0'; //новый ответ
                            }
                            else //чаще встречается буква с отрицанием
                            {
                                replace_vect(Vectors, i, 1, false);
                                result[i] = '1'; //новый ответ
                            }
                        }

                    }
                }

            }
        }
        string algoritm(string[,] Vectors, bool isKNF) //основной алгоритм
        {
            string s = new string('-', Vectors[0, 0].Length);   //ответ в виде строки "----"
            StringBuilder result = new StringBuilder(s); //здесь будет ответ
            while (Checking(Vectors)) //пока есть векторы не нулевые (Нулевой: [0000 0000])
            {
                string[,] Vectors1 = Vectors_1f(Vectors);
                //массив из векторов с одной единицей
                //проверим, что нет противоречий (например: !а & a = 1 для КНФ или !a | a = 0 для ДНФ)
                //надо, чтобы в каждом единичном векторе каждой буквы значения были одинаковы
                //если хотя бы одно значение различно, то нет решений для КНФ = 1 (или для ДНФ = 0)
                while (Vectors1 != null) //если единичные вектора есть
                {
                    for (int i = 0; i < Vectors1[0, 0].Length; i++) //цикл по компоненте в единичных векторах
                    {
                        Boolean flag = true;
                        char value = '-';
                        for (int j = 0; j < Vectors1.GetLength(0); j++) //цикл по векторам
                        {
                            if (Vectors1[j, 0][i].Equals('1')) //если буквы есть, проверяем, совпадают ли значения
                            {
                                if (value == '-') //если значение ещё не было записано
                                {
                                    value = Vectors1[j, 1][i]; //записываем значение первой попавшейся
                                }
                                if (!(Vectors1[j, 1][i].Equals(value))) //если на позиции i символ в значениях не совпадает с value
                                {
                                    flag = false;
                                }
                            }

                        }
                        if (flag == false)
                        {
                            if (isKNF)
                            {
                                return "Нет ответа, КНФ = 0"; //есть единичные векторы с разным значением в одной букве
                            }
                            else
                            {
                                return "Нет ответа, ДНФ = 1"; //есть единичные векторы с разным значением в одной букве
                            }
                        }
                    }
                    for (int i = 0; i < Vectors1[0, 0].Length; i++) //находим ответ для единичных векторов, цикл по компоненте
                    {
                        for (int j = 0; j < Vectors1.GetLength(0); j++) //цикл по векторам
                        {
                            if ((Vectors1[j, 0][i].Equals('1')) && (Vectors1[j, 1][i].Equals('0'))) //если буква есть и она с отрицанием
                            {
                                //функция replace_vect(Vectors, letter, value, isKNF) заменяет все векторы с буквой на позиции letter
                                //на значение value. При этом, если образуется ДНФ=1, то можно убрать вектор из КНФ
                                //Если же образуется КНФ=0, то можно убрать вектор из ДНФ
                                if (isKNF)
                                {
                                    replace_vect(Vectors, i, 0, true); //буква превращается в 0 (результат), буква принимает значение 0 в массиве векторов
                                    result[i] = '0'; //получаем результат для буквы
                                }
                                else
                                {
                                    replace_vect(Vectors, i, 0, false); //буква превращается в 1 (результат), буква принимает значение 1 в массиве векторов
                                    result[i] = '1'; //получаем результат для буквы
                                }

                            }
                            else if ((Vectors1[j, 0][i].Equals('1')) && (Vectors1[j, 1][i].Equals('1'))) //если буква есть и она без отрицания
                            {
                                if (isKNF)
                                {
                                    replace_vect(Vectors, i, 1, true); //буква превращается в 1 (результат)
                                    result[i] = '1'; //получаем результат для буквы
                                }
                                else
                                {
                                    replace_vect(Vectors, i, 0, false); //буква превращается в 0 (результат)
                                    result[i] = '0'; //получаем результат для буквы
                                }

                            }
                        }
                    }
                    Vectors1 = Vectors_1f(Vectors);
                }
                //с единичными разобрались. с помощью жадных алгоритмов ищем следующую букву для замены
                greedy_algoritm(Vectors, isKNF, result);
            }
            return result.ToString();
        }
        string possible_to_replace(string[,] Vectors, string answer, int letter, bool isKNF, string equals)
        {
            StringBuilder result = new StringBuilder(answer); //здесь будет ответ
            string answer1;
            int value = (int)answer[letter];   //значение, которое сейчас в ответе
            int possible_value = Math.Abs(value - 1);   //значение, на которое нужно заменить
            //подставляем все найденные значения, кроме letter
            for (int i = 0; i < answer.Length; i++)
            {
                if ((i != letter) && (!answer[i].Equals('-')))
                {
                    replace_vect(Vectors, i, (int)answer[i], isKNF);
                }
            }
            for (int i = 0; i < Vectors.GetLength(0); i++)  //цикл по векторам
            {
                string m = Vectors[i, 0];
                string p = Vectors[i, 1];
                if ((isKNF) && (equals.Equals("1")))    //КНФ=1
                {
                    if ((m.Count(f => f == '1') == 1) && (m[letter].Equals('1')))   //только одна буква в скобке (заменить не получится)
                        return null;
                    else if ((m.Count(f => f == '1') > 1) && (m[letter].Equals('1')))
                    {                                   //не одна буква => определяем любую другую букву в этой скобке
                        for (int j = 0; j < m.Length; j++)
                        {
                            if ((m[j].Equals('1')) && (p[j].Equals('1')) && (j != letter))
                            {
                                replace_vect(Vectors, j, 1, isKNF);
                                result[j] = '1';
                                break;
                            }
                            if ((m[j].Equals('1')) && (p[j].Equals('0')) && (j != letter))
                            {
                                replace_vect(Vectors, j, 0, isKNF);
                                result[j] = '0';
                                break;
                            }
                        }
                        if ((m[letter].Equals('1')) && (p[letter].Equals('1'))) //буква без отрицания
                        {
                            replace_vect(Vectors, letter, 0, isKNF);    //другой ответ для letter
                            result[letter] = '0';
                        }
                        else if ((m[letter].Equals('1')) && (p[letter].Equals('0'))) //буква с отрицанием
                        {
                            replace_vect(Vectors, letter, 1, isKNF);    //другой ответ для letter
                            result[letter] = '1';
                        }
                    }
                }
                if ((isKNF) && (equals.Equals("0")))    //КНФ=0
                {
                    if ((m.Count(f => f == '1') == 1) && (m[letter].Equals('1')))   //только одна буква в скобке (заменяем скобку на 1)
                    {
                        if (Vectors.GetLength(0) > 1)   //если не 1 вектор в уравнении
                        {
                            if ((m[letter].Equals('1')) && (p[letter].Equals('1'))) //буква без отрицания
                            {
                                replace_vect(Vectors, letter, 1, isKNF);    //другой ответ для letter
                                result[letter] = '1';
                            }
                            else if ((m[letter].Equals('1')) && (p[letter].Equals('0'))) //буква с отрицанием
                            {
                                replace_vect(Vectors, letter, 0, isKNF);    //другой ответ для letter
                                result[letter] = '0';
                            }
                        }
                        else return null;    //заменить не получилось, КНФ будет равна 1 (нет решений уравнения)
                    }
                    else if ((m.Count(f => f == '1') > 1) && (m[letter].Equals('1')))
                    {                                   //не одна буква => определяем любую другую скобку 0, если она не одна
                        if (Vectors.GetLength(0) > 1)   //если не 1 вектор в уравнении
                        {
                            if ((m[letter].Equals('1')) && (p[letter].Equals('1'))) //буква без отрицания
                            {
                                replace_vect(Vectors, letter, 1, isKNF);    //другой ответ для letter
                                result[letter] = '1';
                            }
                            else if ((m[letter].Equals('1')) && (p[letter].Equals('0'))) //буква с отрицанием
                            {
                                replace_vect(Vectors, letter, 0, isKNF);    //другой ответ для letter
                                result[letter] = '0';
                            }
                            if (i != Vectors.GetLength(0) - 1)
                            {
                                answer1 = sub_algoritm(Vectors, isKNF, i + 1);
                            }
                            else answer1 = sub_algoritm(Vectors, isKNF, i - 1);
                            if ((!result[letter].Equals(answer1[letter])) && (!answer1[letter].Equals('-')))    //если ответы для letter не совпадают
                            {
                                return null; //не получилось заменить
                            }
                            for (int j = 0; j < answer1.Length; j++)
                            {
                                if ((!result[j].Equals(answer1[j])) && (!answer1[j].Equals('-')))
                                {
                                    return null;
                                }
                            }
                        }
                        else return null;    //заменить не получилось, КНФ будет равна 1 (нет решений уравнения)
                    }
                }
                if ((!isKNF) && (equals.Equals("0")))    //ДНФ=0
                {
                    if ((m.Count(f => f == '1') == 1) && (m[letter].Equals('1')))   //только одна буква в скобке (заменить не получится)
                        return null;
                    else if ((m.Count(f => f == '1') > 1) && (m[letter].Equals('1')))
                    {                                   //не одна буква => определяем любую другую букву в этой скобке
                        for (int j = 0; j < m.Length; j++)
                        {
                            if ((m[j].Equals('1')) && (p[j].Equals('1')) && (j != letter))
                            {
                                replace_vect(Vectors, j, 0, isKNF);
                                result[j] = '0';
                                break;
                            }
                            if ((m[j].Equals('1')) && (p[j].Equals('0')) && (j != letter))
                            {
                                replace_vect(Vectors, j, 1, isKNF);
                                result[j] = '1';
                                break;
                            }
                        }
                        if ((m[letter].Equals('1')) && (p[letter].Equals('1'))) //буква без отрицания
                        {
                            replace_vect(Vectors, letter, 1, isKNF);    //другой ответ для letter
                            result[letter] = '1';
                        }
                        else if ((m[letter].Equals('1')) && (p[letter].Equals('0'))) //буква с отрицанием
                        {
                            replace_vect(Vectors, letter, 0, isKNF);    //другой ответ для letter
                            result[letter] = '0';
                        }
                    }
                }
                if ((!isKNF) && (equals.Equals("1")))    //ДНФ=1
                {
                    if ((m.Count(f => f == '1') == 1) && (m[letter].Equals('1')))   //только одна буква в скобке (заменяем скобку на 0)
                    {
                        if (Vectors.GetLength(0) > 1)   //если не 1 вектор в уравнении
                        {
                            if ((m[letter].Equals('1')) && (p[letter].Equals('1'))) //буква без отрицания
                            {
                                replace_vect(Vectors, letter, 0, isKNF);    //другой ответ для letter
                                result[letter] = '0';
                            }
                            else if ((m[letter].Equals('1')) && (p[letter].Equals('0'))) //буква с отрицанием
                            {
                                replace_vect(Vectors, letter, 1, isKNF);    //другой ответ для letter
                                result[letter] = '1';
                            }
                        }
                        else return null;    //заменить не получилось, ДНФ будет равна 0 (нет решений уравнения)
                    }
                    else if ((m.Count(f => f == '1') > 1) && (m[letter].Equals('1')))
                    {                                   //не одна буква => определяем любую другую скобку 1, если она не одна
                        if (Vectors.GetLength(0) > 1)   //если не 1 вектор в уравнении
                        {
                            if ((m[letter].Equals('1')) && (p[letter].Equals('1'))) //буква без отрицания
                            {
                                replace_vect(Vectors, letter, 0, isKNF);    //другой ответ для letter
                                result[letter] = '0';
                            }
                            else if ((m[letter].Equals('1')) && (p[letter].Equals('0'))) //буква с отрицанием
                            {
                                replace_vect(Vectors, letter, 1, isKNF);    //другой ответ для letter
                                result[letter] = '1';
                            }
                            if (i != Vectors.GetLength(0) - 1)
                            {
                                answer1 = sub_algoritm(Vectors, isKNF, i + 1);
                            }
                            else answer1 = sub_algoritm(Vectors, isKNF, i - 1);
                            if ((!result[letter].Equals(answer1[letter])) && (!answer1[letter].Equals('-')))    //если ответы для letter не совпадают
                            {
                                return null; //не получилось заменить
                            }
                            for (int j = 0; j < answer1.Length; j++)
                            {
                                if ((!result[j].Equals(answer1[j])) && (!answer1[j].Equals('-')))
                                {
                                    return null;
                                }
                            }
                        }
                        else return null;    //заменить не получилось, ДНФ будет равна 0 (нет решений уравнения)
                    }
                }
            }
            return result.ToString();
        }
        string system_solution(string[] answers)  //решение системы
        {
            string answer1;
            StringBuilder result = new StringBuilder(answers[0]); //изначально решение системы - это решение первого уравнения. будем проверять компоненты в других ответах
            for (int i = 0; i < answers.Length; i++)   //цикл по векторам-ответам, ищем ответ для каждого
            {
                for (int j = 0; j < answers[0].Length; j++)
                {
                    if ((result[j].Equals('-')) && (!answers[i][j].Equals('-')))
                    {
                        result[j] = answers[i][j];
                    }
                    if ((!result[j].Equals('-')) && (!answers[i][j].Equals('-')) && (!answers[i][j].Equals(result[j])))
                    {
                        string s1 = equations[i, 0].Text; //строка первой части уравнения
                        s1 = s1.Replace(" ", ""); //убираем лишние пробелы
                        string[,] Vectors = Solve(s1);
                        string new_result = possible_to_replace(Vectors, answers[i], j, type[i], equations[i, 1].Text);
                        if (new_result == null)
                        {
                            return "Решения системы нет";
                        }
                        else
                        {
                            for (int k = 0; k < result.Length; k++)
                            {
                                if ((result[k].Equals('-')) && (!new_result[k].Equals('-')))
                                {
                                    result[k] = new_result[k];
                                }
                                if (!(result[k].Equals('-')) && (!new_result[k].Equals('-')))
                                {
                                    result[k] = new_result[k];
                                }
                            }
                        }
                    }
                }
                answers[i] = result.ToString();
            }

            return result.ToString();
        }
        string[,] Vectors_1f(string[,] Vectors) //векторы с одной единицей
        {
            int count = 0;
            for (int i = 0; i < Vectors.GetLength(0); i++)
            {
                if (Vectors[i, 0].Count(f => f == '1') == 1)
                {
                    count++;
                }
            }
            string[,] result = new string[count, 2];
            int j = 0;
            for (int i = 0; i < Vectors.GetLength(0); i++)
            {
                if (Vectors[i, 0].Count(f => f == '1') == 1)
                {
                    result[j, 0] = Vectors[i, 0];
                    result[j, 1] = Vectors[i, 1];
                    j++;
                }
            }
            if (count == 0)
                return null;
            else return result;
        }
        bool define_type(string equation)
        {
            equation = equation.Replace(" ", ""); //убираем лишние пробелы
            int countAND = 0;
            int countOR = 0;
            for (int i = 0; i < equation.Length - 1; i++)
            {
                if (equation[i].Equals('&'))
                    countAND++;
                if (equation[i].Equals('|'))
                    countOR++;
                if ((equation[i].Equals('&')) && (equation[i + 1].Equals('(')))
                {
                    return true;
                }
                if ((equation[i].Equals('|')) && (equation[i + 1].Equals('(')))
                {
                    return false;
                }
            }
            if ((countOR == 0) && (countAND > 0))
            {
                return true;
            }
            if ((countOR > 0) && (countAND == 0))
            {
                return false;
            }
            return true;
        }
        Boolean Checking(string[,] Vectors) //проверка наличия ненулевых векторов
        {
            int count = 0;
            for (int i = 0; i < Vectors.GetLength(0); i++)
            {
                string s = new string('0', Vectors[i, 0].Length);
                string m = Vectors[i, 0];
                if (!m.Equals(s))
                {
                    count++;
                }
            }
            if (count == 0)
            {
                return false;
            }
            else return true;
        }
        string[,] Solve(string s) //функция преобразования строки в массив "векторов" [1110, 0010], [1011, 1000] и т.д. 
        {
            var form = new Form1();
            int count1 = 0;
            bool isKNF = define_type(s); //надо понять: КНФ или ДНФ
            for (int i = 0; i < s.Length; i++) //выясняем количество множителей (слагаемых) в левой части
            {
                if (((s[i] == '&') && (isKNF)) || (((s[i] == '|') && !(isKNF))))
                    count1++;
            }
            string[] vect = new string[count1 + 1]; //массив строк, каждая является содержимым закрытых скобок
            int count2 = 0;
            int t = 0;
            string str = "";
            string Alphabet = "!AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz";
            while (t < s.Length)
            {
                if (s[t] == '(') //начиная с открытой скобки записывать содержимое строки
                {
                    while (s[t] != ')') //пока не встретили закрытую скобку, прибавляем символы к str
                    {
                        str += s[t];
                        t++;
                    }
                    vect[count2] = str; //записываем элемент массива
                    count2++;
                    str = ""; //очищаем str
                }
                else if (Alphabet.Contains(s[t]))
                {
                    while (Alphabet.Contains(s[t])) //для одиночных множителей, прибавляем символы к str
                    {
                        str += s[t];
                        t++;
                    }
                    vect[count2] = str; //записываем элемент массива
                    count2++;
                    str = ""; //очищаем str
                }
                t++;
            }
            string[,] A = new string[vect.Length, 2]; //двумерный массив (вектор 1 и вектор 2 соответственно)
            for (int i = 0; i < vect.Length; i++) //преобразуем в булевы вектора (Vector1(vect[i]) и Vector2(vect[i]))
            {
                A[i, 0] = form.Vector1(vect[i]); //вектор из 0 и 1, определяющий наличие пропозициональной буквы
                A[i, 1] = form.Vector2(vect[i]); //вектор из 0 и 1, определяющий вхождение буквы с отрицанием или без
            }
            return A;
        }
        static void replace_vect(string[,] Vectors, int letter, int value, bool isKNF)
        {
            //определить: КНФ или ДНФ
            if (isKNF)
            {
                for (int i = 0; i < Vectors.GetLength(0); i++)    //проходим по всем массивам векторов [1110, 0010], [1011, 1000] ...
                {
                    StringBuilder m = new StringBuilder(Vectors[i, 0]);
                    StringBuilder p = new StringBuilder(Vectors[i, 1]);
                    char ch = (char)value;
                    if ((value == 0) & (m[letter] == '1'))      //если надо заменить на 0 и буква есть
                    {
                        if (p[letter] == '1') //если буква без отрицания (0 == 0)
                        {
                            m[letter] = '0';   //заменяем m[letter] и p[letter] на 0
                            p[letter] = '0';
                        }
                        else if (p[letter] == '0') //если буква с отрицанием (!0 == 1)
                        {
                            for (int j = 0; j < m.Length; j++)        //обнуляем вектор (так как ДНФ станет истиной)
                            {
                                m[j] = '0';
                                p[j] = '0';
                            }
                        }

                    }
                    else if ((value == 1) & (m[letter] == '1')) //если надо заменить на 1 и буква есть
                    {
                        if (p[letter] == '1') //если буква без отрицания (1 == 1)
                        {
                            for (int j = 0; j < m.Length; j++)        //обнуляем вектор (так как ДНФ станет истиной)
                            {
                                m[j] = '0';
                                p[j] = '0';
                            }

                        }
                        else if (p[letter] == '0') //если буква с отрицанием (!1 == 0)
                        {
                            m[letter] = '0';        //заменяем m[letter] и p[letter] на 0
                            p[letter] = '0';
                        }
                    }
                    Vectors[i, 0] = m.ToString();
                    Vectors[i, 1] = p.ToString();
                }
            }
            else
            {
                for (int i = 0; i < Vectors.GetLength(0); i++)    //проходим по всем массивам векторов [1110, 0010], [1011, 1000] ...
                {
                    StringBuilder m = new StringBuilder(Vectors[i, 0]);
                    StringBuilder p = new StringBuilder(Vectors[i, 1]);
                    char ch = (char)value;
                    if ((value == 1) & (m[letter] == '1'))      //если надо заменить на 1 и буква есть
                    {
                        if (p[letter] == '1') //если буква без отрицания (1 == 1)
                        {
                            m[letter] = '0';   //заменяем m[letter] и p[letter] на 0
                            p[letter] = '0';
                        }
                        else if (p[letter] == '0') //если буква с отрицанием (!1 == 0)
                        {
                            for (int j = 0; j < m.Length; j++)        //обнуляем вектор (так как КНФ станет ложью)
                            {
                                m[j] = '0';
                                p[j] = '0';
                            }
                        }

                    }
                    else if ((value == 0) & (m[letter] == '1')) //если надо заменить на 0 и буква есть
                    {
                        if (p[letter] == '1') //если буква без отрицания (0 == 0)
                        {
                            for (int j = 0; j < m.Length; j++)        //обнуляем вектор (так как КНФ станет ложью)
                            {
                                m[j] = '0';
                                p[j] = '0';
                            }

                        }
                        else if (p[letter] == '0') //если буква с отрицанием (!0 == 1)
                        {
                            m[letter] = '0';        //заменяем m[letter] и p[letter] на 0
                            p[letter] = '0';
                        }
                    }
                    Vectors[i, 0] = m.ToString();
                    Vectors[i, 1] = p.ToString();
                }
            }


        }
        string Vector1(string s)
        {
            if (s.Equals(null))
            {
                label1.Text = "Нет данных";
            }
            string a = "";
            string Alphabet = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz";
            for (int i = 0; i < Alphabet.Length - 1; i += 2)
            {
                if (s.Contains(Alphabet[i].ToString()) || s.Contains(Alphabet[i + 1].ToString()))
                {
                    a += "1";
                }
                else a += '0';
            }
            return a;
        }   //маска (вектор наличия букв)
        string Vector2(string s)
        {
            string Alphabet = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz";
            string a = "";
            for (int i = 0; i < Alphabet.Length - 1; i += 2)
            {
                if (s.Contains("!" + Alphabet[i].ToString()) || s.Contains("!" + Alphabet[i + 1].ToString()))
                {
                    a += "0";
                }
                else if (s.Contains(Alphabet[i].ToString()) || s.Contains(Alphabet[i + 1].ToString()))
                {
                    a += '1';
                }
                else a += '0';
            }
            return a;
        }   //вектор значений

        void starting_controls()
        {
            //кнопка 1
            {
                Button button1 = new Button();
                button1.Name = "button1";
                button1.Text = "Решить";
                button1.Location = new Point(38, 300);
                button1.Size = new Size(100, 26);
                button1.Click += button1_Click;
                this.Controls.Add(button1);
            }
            //кнопка 2
            {
                Button button2 = new Button();
                button2.Name = "button2";
                button2.Text = "Синтез тестов";
                button2.Location = new Point(38, 320);
                button2.Size = new Size(100, 26);
                button2.Click += button2_Click;
                this.Controls.Add(button2);
            }
            //кнопка 3
            {
                Button button3 = new Button();
                button3.Name = "button3";
                button3.Text = "Сгенерировать";
                button3.Location = new Point(138, 300);
                button3.Size = new Size(100, 26);
                button3.Click += button3_Click;
                this.Controls.Add(button3);
            }
            //метка 1
            {
                Label lab = new Label();
                Font font = new Font("Microsoft Sans Serif", 9.5f);
                lab.Name = "label1";
                lab.Text = "Вывод ответа";
                lab.Location = new Point(400, 10);
                lab.Size = new Size(500, 2000);
                lab.Font = font;
                lab.UseMnemonic = false;
                label1 = lab;
                this.Controls.Add(label1);
            }
            //метка 3
            {
                Label lab = new Label();
                lab.Name = "label3";
                lab.Text = "Число внешних";
                lab.Location = new Point(38, 350);
                lab.Size = new Size(140, 20);
                label3 = lab;
                this.Controls.Add(label3);
            }
            //метка 4
            {
                Label lab = new Label();
                lab.Name = "label4";
                lab.Text = "Число внутренних (max)";
                lab.Location = new Point(38, 370);
                lab.Size = new Size(140, 20);
                label4 = lab;
                this.Controls.Add(label4);
            }
            //метка 5
            {
                Label lab = new Label();
                lab.Name = "label5";
                lab.Text = "Число уравнений";
                lab.Location = new Point(38, 390);
                lab.Size = new Size(140, 20);
                label5 = lab;
                this.Controls.Add(label5);
            }
            //текстовое поле 2
            {
                TextBox txt = new TextBox();
                txt.Name = "textBox2";
                txt.Text = k.ToString();
                txt.Location = new Point(180, 350);
                textBox2 = txt;
                this.Controls.Add(textBox2);
            }
            //текстовое поле 3
            {
                TextBox txt = new TextBox();
                txt.Name = "textBox3";
                txt.Text = n.ToString();
                txt.Location = new Point(180, 370);
                textBox3 = txt;
                this.Controls.Add(textBox3);
            }
            //текстовое поле 5
            {
                TextBox txt = new TextBox();
                txt.Name = "textBox5";
                txt.Text = equation_number.ToString();
                txt.Location = new Point(180, 390);
                textBox5 = txt;
                this.Controls.Add(textBox5);
            }
        }

        public string[] synthesis_of_tests(string[] answers)
        {
            string[] res = new string[equation_number];
            for (int i = 0; i < equations.GetLength(0); i++)
            {
                StringBuilder s1 = new StringBuilder(equations[i, 0].Text); //строка первой части уравнения
                StringBuilder s2 = new StringBuilder(answers[i]);
                value_equals = equations[i, 1].Text;
                string Alphabet = "abcdefghijklmnopqrstuvwxyz";
                for (int j = 0; j < s1.Length; j++)
                {
                    if (Alphabet.Contains(s1[j]))
                    {
                        for (int k = 0; k < Alphabet.Length; k++)
                        {
                            char letter = Alphabet[k];
                            char answer = s2[k];
                            if (s1[j].Equals(letter))
                            {
                                if (!answer.Equals('-'))
                                {
                                    s1[j] = answer;
                                }
                                else
                                {
                                    s1[j] = '1';
                                }
                            }
                        }
                    }
                }
                res[i] = s1.ToString();
            }
            return res;
        }
        public string[] solving(string[] equats)
        {
            string[] result = new string[equation_number];
            string values = "01";
            StringBuilder vector = new StringBuilder("");
            for (int i = 0; i < equation_number; i++)
            {
                int t = 0;
                while (t < equats[i].Length)
                {
                    char ch1 = '-';
                    if (t > 0)
                    {
                        ch1 = equats[i][t - 1];
                    }
                    char ch2 = equats[i][t];
                    StringBuilder str = new StringBuilder("");
                    if (ch2 == '(') //начиная с открытой скобки
                    {
                        while (ch2 != ')') //пока не встретили закрытую скобку
                        {
                            if (ch1 != '!')
                            {
                                if ((ch2 == '0') || (ch2 == '1'))
                                {
                                    str.Append(ch2);
                                }

                            }
                            if (ch1 == '!')
                            {
                                if (ch2 == '0')
                                {
                                    str.Append('1');
                                }
                                if (ch2 == '1')
                                {
                                    str.Append('0');
                                }

                            }
                            t++;
                            ch1 = equats[i][t - 1];
                            ch2 = equats[i][t];
                        }
                    }
                    if (type[i] == true)
                    {
                        if (str.Length > 0)
                        {
                            if (str.ToString().Equals(new string('0', str.Length)))
                            {
                                vector.Append('0');
                            }
                            else
                            {
                                vector.Append('1');
                            }

                        }

                    }
                    if (type[i] == false)
                    {
                        if (str.Length > 0)
                        {
                            if (str.ToString().Equals(new string('1', str.Length)))
                            {
                                vector.Append('1');
                            }
                            else
                            {
                                vector.Append('0');
                            }

                        }

                    }
                    t++;
                }

                if (type[i] == true) //КНФ=1 (в каждой дизъюнкции хотя бы одна 1) || КНФ=0 (хотя бы одна дизъюнкция 0)
                {
                    if (vector.ToString().Equals(new string('1', vector.Length)))
                    {
                        result[i] = "1";
                    }
                    else
                    {
                        result[i] = "0";
                    }
                }
                if (type[i] == false)   //ДНФ=0 (в каждой конъюнкции хотя бы один 0) || ДНФ=1 (хотя бы одна конъюнкция 1)
                {
                    if (vector.ToString().Equals(new string('0', vector.Length)))
                    {
                        result[i] = "0";
                    }
                    else
                    {
                        result[i] = "1";
                    }
                }
                vector = new StringBuilder("");
            }
            return result;
        }
        private void button1_Click(object sender, EventArgs e)  //кнопка "решить"
        {
            try
            {
                if (equations == null)
                {
                    throw new ArgumentNullException();
                }
                answers = new string[equations.GetLength(0)];
                for (int i = 0; i < equations.GetLength(0); i++)
                {
                    string s1 = equations[i, 0].Text; //строка первой части уравнения
                    label1.Text = "";
                    s1 = s1.Replace(" ", ""); //убираем лишние пробелы
                    value_equals = equations[i, 1].Text;
                    //Пусть будет КНФ=(value_equals) или ДНФ=(value_equals)
                    string[,] Vectors = Solve(s1); //из строки получаем массив из массивов 2-х векторов [[1000 0000] и [0010 0010]]
                    type[i] = define_type(s1);
                    string answer;
                    if (((type[i] == true) && (value_equals.Equals("1"))) || ((type[i] == false) && (value_equals.Equals("0"))))
                    {
                        answer = algoritm(Vectors, type[i]); //основной алгоритм для КНФ=1 и ДНФ=0
                    }
                    else
                    {
                        answer = sub_algoritm(Vectors, type[i], 0); //алгоритм для КНФ=0 и ДНФ=1
                    }
                    answers[i] = answer;
                }
                string Alphabet = "abcdefghijklmnopqrstuvwxyz";
                int k;
                for (int i = 0; i < answers.Length; i++)
                {
                    label1.Text += "Уравнение " + (i+1) + Environment.NewLine;
                    k = 0;
                    //вывод ответа
                    if ((answers[i].Equals("Нет ответа, КНФ = 0")) || (answers[i].Equals("Нет ответа, ДНФ = 1")))
                    {
                        label1.Text += answers[i];
                    }
                    else
                        for (int j = 0; j < answers[i].Length; j++)
                        {
                            if (!answers[i][j].Equals('-'))
                            {
                                label1.Text += Alphabet[k].ToString() + " = " + answers[i][j].ToString() + " ";
                            }
                            k++;
                        }
                    label1.Text += Environment.NewLine;
                }
                if (answers.GetLength(0) > 1)
                {
                    string system_answer = system_solution(answers);
                    k = 0;
                    if (system_answer.Equals("Решения системы нет"))
                    {
                        label1.Text += system_answer + Environment.NewLine;
                    }
                    else
                    {
                        label1.Text += "Решение системы: " + Environment.NewLine;
                        for (int j = 0; j < system_answer.Length; j++)
                        {
                            if (!system_answer[j].Equals('-'))
                            {
                                label1.Text += Alphabet[k].ToString() + " = " + system_answer[j].ToString() + " ";
                            }
                            k++;
                        }
                        label1.Text += Environment.NewLine + "Остальные буквы могут быть любыми, если они есть." + Environment.NewLine;
                    }
                }                
            }
            catch (ArgumentNullException)
            {
                label1.Text = "Уравнения не введены.";
            }
            
            
        }

        private void button3_Click(object sender, EventArgs e)  //кнопка "сгенерировать"
        {
            try
            {
                if (!(int.TryParse(textBox2.Text, out k)) || !(int.TryParse(textBox3.Text, out n)) || !(int.TryParse(textBox5.Text, out equation_number)))
                {
                    throw new FormatException();
                }
                else
                {
                    Font font = new Font("Microsoft Sans Serif", 14.0f);

                    this.Controls.Clear();
                    starting_controls();
                    type = new bool[equation_number];
                    equations = new TextBox[equation_number, 3]; //массив TextBox (уравнение, правая часть, тип (КНФ-1 или ДНФ-0))
                    for (int i = 0; i < equation_number; i++)
                    {
                        String name = "AdditionalLabel" + (i + 1).ToString();
                        string[] generation = generate(k, n);
                        TextBox txt = new TextBox();
                        Label lab = new Label();
                        lab.Text = "=";
                        lab.Location = new Point(340, i * 30);
                        lab.Size = new Size(10, 26);
                        lab.Name = name;
                        lab.Font = font;
                        this.Controls.Add(lab);
                        name = "Equation" + (i + 1).ToString();
                        txt.Name = name;
                        txt.Location = new Point(38, i * 30);
                        txt.Size = new Size(300, 26);
                        txt.Text = name;
                        txt.Font = font;
                        equations[i, 0] = txt;
                        equations[i, 0].Text = generation[0];
                        if (generation[1].Equals("1"))
                            type[i] = true;
                        else type[i] = false;
                        this.Controls.Add(equations[i, 0]);
                        name = "Equals" + (i + 1).ToString();
                        txt = new TextBox();
                        txt.Name = name;
                        txt.Location = new Point(350, i * 30);
                        txt.Size = new Size(23, 26);
                        txt.Font = font;
                        equations[i, 1] = txt;
                        equations[i, 1].Text = new Random().Next(0, 2).ToString();
                        this.Controls.Add(equations[i, 1]);
                        this.Refresh();
                    }
                }
            }
            catch (FormatException)
            {
                label1.Text = "Поля имеют неправильные вводные значения.";
            }
            
        }

        private void button2_Click(object sender, EventArgs e)  //кнопка "синтез тестов"
        {
            try
            {
                if (equations == null)
                {
                    throw new ArgumentNullException();
                }
                string system_answer = system_solution(answers);
                if (!system_answer.Equals("Решения системы нет"))
                {
                    label1.Text += "Синтез тестов: " + Environment.NewLine;
                    string[] s = synthesis_of_tests(answers);
                    string[] s1 = solving(s);
                    for (int i = 0; i < s.Length; i++)
                    {
                        label1.Text += s[i] + "=" + s1[i] + Environment.NewLine;
                    }
                }
            }
            catch (ArgumentNullException)
            {
                label1.Text = "Невозможно провести синтез тестов. Нет уравнений." + Environment.NewLine;
            }
            
        }
    }
}