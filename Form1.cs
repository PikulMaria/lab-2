using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace labFour
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }
        
        private void btnSelectFolder_Click(object sender, EventArgs e)//Выбор папки с файлами
        {
            textBox1.Text = "";
            if (folderBrowserDialog2.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog2.SelectedPath;
            }
        }

        private void btnSelectCopyPath_Click(object sender, EventArgs e)//Выбор папки для копирования
        {
            textBox2.Text = "";
            if (folderBrowserDialog2.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog2.SelectedPath;
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {// изменение процесса backgroundWorker
            progressBar1.Value = e.ProgressPercentage;//Возвращает процент ассинхронной задачи
            label3.Text = "Процес... "+progressBar1.Value.ToString() + "%";
        }
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            CopyFiles(textBox1.Text, textBox2.Text);
           
        }
                        
            private void btnStart_Click(object sender, EventArgs e)
        {
            try { 
                backgroundWorker.RunWorkerAsync();
               
            }
            catch (Exception exeption) {
                MessageBox.Show("Произошла ошибка!" + exeption.Message);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try { 
                backgroundWorker.CancelAsync(); 
            }
            catch (Exception exeption)
            {
                MessageBox.Show("Произошла ошибка!" + exeption.Message);
            }
        }

        private void CopyFiles(string source, string dest)
        { // Копирование папки
            try
            {
                System.IO.Directory.CreateDirectory(dest);
                //Выполняет указанный делегат в том потоке, которому принадлежит базовый дескриптор окна элемента управления
                progressBar2.Invoke(new Action(() => progressBar2.Minimum = 0));
                progressBar2.Invoke(new Action(() => progressBar2.Maximum = Directory.GetFiles(source).Length));
                progressBar2.Invoke(new Action(() => progressBar2.Value = 0));
                progressBar2.Invoke(new Action(() => progressBar2.Step = 1));

                float posPercent;
                string filename, destFile;
                foreach (string s in Directory.GetFiles(source))//Directory.GetFiles-Возвращает имена файлов, соответствующих указанным критериям.
                {
                    if (backgroundWorker.CancellationPending == true)//запросило ли приложение отмену фоновой операции?
                    {
                        return;
                    }
                    filename = Path.GetFileName(s);//Возвращает имя файла и расширение указанной строки пути
                    destFile = Path.Combine(dest, filename);//Объединяет строки в путь
                   
                    label4.Invoke(new Action(() => label4.Text = s));
                    FileStream fsOut = new FileStream(destFile, FileMode.Create);//создается новый файл. Если такой файл уже существует, то он перезаписывается
                    //Предоставляет Stream в файле, поддерживая синхронные и асинхронные операции чтения и записи
                    //Stream - Предоставляет универсальное представление последовательности байтов. 
                    FileStream fsIn = new FileStream(s, FileMode.Open);// открывает файл

                    byte[] bt = new byte[1048756]; // Один мегабайт
                    int readByte;
                    while ((readByte = fsIn.Read(bt, 0, bt.Length)) > 0)//Выполняет чтение блока байтов из потока и запись данных в заданный буфер.
                    {
                        fsOut.Write(bt, 0, readByte);//Записывает блок байтов в файловый поток
                        backgroundWorker.ReportProgress((int)(fsIn.Position * 100 / fsIn.Length));
                        if (backgroundWorker.CancellationPending == true)
                        {
                            fsIn.Close();
                            fsOut.Close();
                            File.Delete(destFile);
                            label3.Invoke(new Action(() => label3.Text = "Отменено"));
                            return;
                        }
                    }
                    fsIn.Close();
                    fsOut.Close();
                    label3.Invoke(new Action(() => label3.Text = "Успешно"));
                    
                    progressBar2.Invoke(new Action(() => progressBar2.PerformStep()));
                    posPercent = 100 * progressBar2.Value / progressBar2.Maximum;
                    label7.Invoke(new Action(() => label7.Text = "Процесс..." + posPercent.ToString() + "%"));
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка!...\n\n" +
                    ex.Message);
            }
        }
        
    }

}
