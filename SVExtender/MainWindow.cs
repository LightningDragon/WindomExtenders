using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SVExtender.Properties;
using System.Windows.Forms;
using static SVExtender.Helper;


namespace SVExtender
{
    public sealed partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            Text = $"{Application.ProductName} {Application.ProductVersion}";
            RoboCheckBox.Checked = Settings.Default.UseFolderInsted;
            ExtendCheckBox.Checked = Settings.Default.Extend;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            const string exeName = "WindomSV_LV.exe";

            ProcessWrapper process = new ProcessWrapper();

            try
            {
                if (!ExtendCheckBox.Checked)
                {
                    if (!process.Start(exeName))
                    {
                        MessageBox.Show($"Unable to start {exeName}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    if (process.Start($"{exeName}", null, 0x4))
                    {
                        byte[][] maps = (from l in MapCheckBox.Checked ? Directory.GetDirectories("map") : File.ReadAllLines("Map.txt") where !l.IsNullOrWhiteSpace() select ShiftJis.GetBytes(l)).ToArray();
                        byte[][] robots = (from l in RoboCheckBox.Checked ? Directory.GetDirectories("Robo") : File.ReadAllLines("Robo.txt") where !l.IsNullOrWhiteSpace() select ShiftJis.GetBytes(l)).ToArray();
                        byte[] mapsPaths = maps.SelectMany(x => x.Concat(NullTerminator)).ToArray();
                        byte[] robotPaths = robots.SelectMany(x => x.Concat(NullTerminator)).ToArray();
                        byte[] loop = Resources.forloop;

                        IntPtr numberOfMechaPtr = ErrorCheck(process.Alloc(4));
                        IntPtr numberOfMapPtr = ErrorCheck(process.Alloc(4));
                        IntPtr roboPathsPtr = ErrorCheck(process.Alloc((uint)robotPaths.Length));
                        IntPtr mapPathsPtr = ErrorCheck(process.Alloc((uint)mapsPaths.Length));
                        IntPtr roboPathsPtrPtr = ErrorCheck(process.Alloc((uint)robots.Length * 4));
                        IntPtr mapPathsPtrPtr = ErrorCheck(process.Alloc((uint)maps.Length * 4));
                        IntPtr forLoopFuncPtr = ErrorCheck(process.Alloc((uint)loop.Length));

                        process.Write(numberOfMechaPtr, BitConverter.GetBytes(robots.Length), 4);
                        process.Write(numberOfMapPtr, BitConverter.GetBytes(maps.Length), 4);
                        process.Write(roboPathsPtr, robotPaths, robotPaths.Length);
                        process.Write(mapPathsPtr, mapsPaths, mapsPaths.Length);

                        {
                            int position;
                            int index;
                            for (index = 0, position = 0; index < maps.Length; index++)
                            {
                                process.Write(mapPathsPtrPtr + 4 * index, BitConverter.GetBytes((uint)(mapPathsPtr + position)), 4);
                                position += maps[index].Length + 1;
                            }
                        }

                        {
                            int position;
                            int index;
                            for (index = 0, position = 0; index < robots.Length; index++)
                            {
                                process.Write(roboPathsPtrPtr + 4 * index, BitConverter.GetBytes((uint)(roboPathsPtr + position)), 4);
                                position += robots[index].Length + 1;
                            }
                        }

                        ErrorCheck(process.Resume());
                        ErrorCheck(process.Suspend());

                        IntPtr baseAddress = Process.GetProcessById(process.ProcessInfo.dwProcessId).MainModule.BaseAddress;

                        // Unlocks all robots
                        process.Write(baseAddress + 0xC3C0C, BitConverter.GetBytes(1), 1);

                        Buffer.BlockCopy(BitConverter.GetBytes((uint)baseAddress + 0x850D0), 0, loop, 0x21, 4);
                        Buffer.BlockCopy(BitConverter.GetBytes((uint)roboPathsPtrPtr), 0, loop, 0xD, 4);
                        Buffer.BlockCopy(BitConverter.GetBytes((uint)numberOfMechaPtr), 0, loop, 0x2C, 4);
                        process.Write(forLoopFuncPtr, loop, loop.Length);
                        // TODO: Extend nops to cover maps
                        process.WriteNops(baseAddress + 0x85058, 0x41);
                        process.WriteCall(baseAddress + 0x85058, forLoopFuncPtr);

                        ErrorCheck(process.Resume());
                    }
                    else
                    {
                        MessageBox.Show($"Unable to start {exeName}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                process.Exit(0);
            }
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.UseFolderInsted = RoboCheckBox.Checked;
            Settings.Default.Extend = ExtendCheckBox.Checked;
            Settings.Default.Save();
        }
    }
}
