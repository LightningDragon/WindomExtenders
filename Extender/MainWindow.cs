using System;
using System.IO;
using System.Linq;
using Extender.Properties;
using System.Windows.Forms;
using static Extender.Helper;


namespace Extender
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            Text = $"{Application.ProductName} {Application.ProductVersion}";
            RoboCheckBox.Checked = Settings.Default.UseFolderInsted;
            ExtendCheckBox.Checked = Settings.Default.Extend;
        }

        private unsafe void button1_Click(object sender, EventArgs e)
        {
            const byte jmp = 0xEB;
            const byte lea = 0x8D;
            const byte cmp = 0x81;
            const byte mov = 0xC7;
            const byte otherMov = 0x89;
            const byte otherOtherMov = 0x8B;
            const int sizeOfD3DTextureThing = 0x48;
            const int sizeOfMechaInfo = 0x264;
            const int SizeOfHangarMechaInfo = 0x25C;
            const int sizeOfMechaPath = 100;
            const int sizeOfSaveEntery = 0x38;

            ProcessWrapper process = new ProcessWrapper();

            try
            {
                if (!ExtendCheckBox.Checked)
                {
                    if (!process.Start("WindomXP.exe"))
                    {
                        MessageBox.Show("Unable to start WindomXP.exe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    if (process.Start("WindomXP.exe", null, 0x4))
                    {
                        byte[][] mechaPaths = (RoboCheckBox.Checked ? (from d in Directory.GetDirectories("Robo") select shift_jis.GetBytes(d).Resize(sizeOfMechaPath)) : (from l in File.ReadAllLines("Robo.txt") where !l.IsNullOrWhiteSpace() select shift_jis.GetBytes(l).Resize(sizeOfMechaPath))).ToArray();

                        uint numberOfMecha = (uint)mechaPaths.Length;

                        IntPtr face2TexturesPtr = ErrorCheck(process.Alloc(sizeOfD3DTextureThing * (numberOfMecha * 2)));
                        IntPtr texturesPtr = ErrorCheck(process.Alloc(sizeOfD3DTextureThing * numberOfMecha));
                        IntPtr mechaInfosPtr = ErrorCheck(process.Alloc(sizeOfMechaInfo * numberOfMecha));
                        IntPtr hangarmechaInfosPtr = ErrorCheck(process.Alloc(SizeOfHangarMechaInfo * numberOfMecha));
                        IntPtr mechaPathsPtr = ErrorCheck(process.Alloc(sizeOfMechaPath * numberOfMecha));
                        IntPtr saveEnteryPtr = ErrorCheck(process.Alloc(sizeOfSaveEntery * Math.Max(100, numberOfMecha)));

                        for (int index = 0; index < numberOfMecha; index++)
                        {
                            ErrorCheck(process.Write(mechaPathsPtr + (sizeOfMechaPath * index), mechaPaths[index], sizeOfMechaPath));
                        }

                        byte[] blackSaveEntery = Enumerable.Repeat<byte>(1, sizeOfSaveEntery).ToArray();

                        for (int index = 0; index < numberOfMecha; index++)
                        {
                            ErrorCheck(process.Write(saveEnteryPtr + (sizeOfSaveEntery * index), blackSaveEntery, sizeOfSaveEntery));
                        }

                        // Sets the number of Mecha In the MainClass
                        ErrorCheck(process.Write((IntPtr)0x4021B1, &numberOfMecha, 4));

                        // Replaces the arrays used for the character select thumbnails, because they're limited to 50 by default
                        process.WriteInstruction(lea, 0x8A, texturesPtr, 0x5188F8, 7);
                        process.WriteInstruction(lea, 0x8A, texturesPtr, 0x518916, 7);

                        // Replaces the arrays used for the mecha folder locations
                        process.WriteInstruction(lea, 0x91, mechaPathsPtr, 0x499401, 7);
                        process.WriteInstruction(lea, 0x91, mechaPathsPtr, 0x518810, 7);
                        process.WriteInstruction(lea, 0x91, mechaPathsPtr, 0x518CB5, 7);
                        process.WriteInstruction(lea, 0x91, mechaPathsPtr, 0x51C824, 7);
                        process.WriteInstruction(lea, 0x82, mechaPathsPtr, 0x51C4F9, 7);

                        // Skips unlock checks
                        process.WriteNops(0x518686, 4);
                        process.WriteNops(0x518755, 8);

                        //
                        process.WriteInstruction(lea, 0x81, mechaInfosPtr + 0x4, 0x5189B8, 7);
                        process.WriteInstruction(lea, 0x90, mechaInfosPtr + 0x68, 0x5189E3, 7);
                        process.WriteInstruction(lea, 0x8A, mechaInfosPtr + 0xCC, 0x518A0E, 7);
                        process.WriteInstruction(lea, 0x81, mechaInfosPtr + 0x130, 0x518A39, 7);
                        process.WriteInstruction(lea, 0x90, mechaInfosPtr + 0x194, 0x518A64, 7);
                        process.WriteInstruction(lea, 0x8A, mechaInfosPtr + 0x1F8, 0x518A8F, 7);

                        process.WriteInstruction(lea, 0x90, mechaInfosPtr + 0x260, 0x518AB4, 7);

                        process.WriteInstruction(lea, 0x81, mechaInfosPtr + 0x25C, 0x518AD1, 7);
                        process.WriteInstruction(lea, 0x90, mechaInfosPtr + 0x4, 0x51EF38, 7);
                        process.WriteInstruction(lea, 0x90, mechaInfosPtr + 0x4, 0x51F023, 7);
                        process.WriteInstruction(lea, 0x90, mechaInfosPtr + 0x4, 0x51F10B, 7);
                        process.WriteInstruction(lea, 0x90, mechaInfosPtr + 0x4, 0x51F1F6, 7);
                        process.WriteInstruction(lea, 0x90, mechaInfosPtr + 0x4, 0x51F2E1, 7);
                        process.WriteInstruction(lea, 0x81, mechaInfosPtr + 0x68, 0x51F348, 7);
                        process.WriteInstruction(lea, 0x81, mechaInfosPtr + 0xCC, 0x51F421, 7);
                        process.WriteInstruction(lea, 0x81, mechaInfosPtr + 0x130, 0x51F4FA, 7);
                        process.WriteInstruction(lea, 0x81, mechaInfosPtr + 0x194, 0x51F5D3, 7);
                        process.WriteInstruction(lea, 0x81, mechaInfosPtr + 0x1F8, 0x51F6AC, 7);
                        process.WriteInstruction(otherMov, 0x8A, mechaInfosPtr, 0x518977, 7);
                        process.WriteInstruction(otherOtherMov, 0x89, mechaInfosPtr, 0x51C248, 7);
                        process.WriteInstruction(otherOtherMov, 0x92, mechaInfosPtr, 0x51C4EF, 7);
                        process.WriteInstruction(otherOtherMov, 0x89, mechaInfosPtr, 0x51C81A, 7);
                        process.WriteInstruction(otherOtherMov, 0x92, mechaInfosPtr, 0x520EDF, 7);

                        #region Online stuff

                        process.WriteInstruction(lea, 0x88, texturesPtr, 0x522462, 7);
                        process.WriteInstruction(lea, 0x90, texturesPtr, 0x52762E, 7);
                        process.WriteInstruction(lea, 0x90, texturesPtr, 0x52764C, 7);

                        process.WriteInstruction(lea, 0x82, mechaPathsPtr, 0x522403, 7);

                        // Skip the over 100 check
                        process.WriteByte((IntPtr)0x524EFF, jmp);
                        process.WriteByte((IntPtr)0x524E94, jmp);
                        process.WriteByte((IntPtr)0x522516, jmp);

                        //
                        {
                            byte* data = stackalloc byte[5];
                            data[0] = 0xB8;
                            *((uint*)&data[1]) = (uint)saveEnteryPtr;

                            ErrorCheck(process.Write((IntPtr)0x5224EC, data, 5));
                            ErrorCheck(process.Write((IntPtr)0x524E76, data, 5));
                            ErrorCheck(process.Write((IntPtr)0x524EE1, data, 5));
                            ErrorCheck(process.Write((IntPtr)0x524F86, data, 5));
                            ErrorCheck(process.Write((IntPtr)0x524FEA, data, 5));
                        }

                        {
                            byte* data = stackalloc byte[7];
                            data[0] = 0xC7;
                            data[1] = 0x45;
                            data[2] = 0xFC;
                            *((uint*)&data[3]) = (uint)saveEnteryPtr;

                            process.WriteNops(0x524E76, 11);
                            process.Write((IntPtr)0x524E76, data, 7);
                        }

                        {
                            byte* data = stackalloc byte[7];
                            data[0] = 0xC7;
                            data[1] = 0x45;
                            data[2] = 0xF0;
                            *((uint*)&data[3]) = (uint)saveEnteryPtr;

                            process.WriteNops(0x524EE1, 11);
                            process.Write((IntPtr)0x524EE1, data, 7);
                        }

                        {
                            byte* data = stackalloc byte[10];
                            data[0] = 0xC7;
                            data[1] = 0x85;
                            data[2] = 0xC0;
                            data[3] = 0xFD;
                            data[4] = 0xFF;
                            data[5] = 0xFF;
                            *((uint*)&data[6]) = (uint)saveEnteryPtr;

                            process.WriteNops(0x5224EC, 11);
                            process.Write((IntPtr)0x5224EC, data, 10);
                        }

                        #endregion

                        #region RoomScene

                        //
                        process.WriteInstruction(lea, 0x82, mechaPathsPtr, 0x52B467, 7);
                        process.WriteInstruction(lea, 0x86, mechaPathsPtr, 0x52B2A5, 7);

                        //
                        process.WriteInstruction(lea, 0x88, face2TexturesPtr, 0x52B4C6, 7);
                        process.WriteInstruction(lea, 0x90, face2TexturesPtr + 0x2C, 0x53488A, 7);

                        //
                        process.WriteInstruction(otherOtherMov, 0x90, face2TexturesPtr, 0x5348D0, 7);
                        
                        #endregion

                        #region Hangar stuff
                        //
                        process.WriteNops(0x53E0D0, 8);

                        //
                        process.WriteInstruction(lea, 0x91, mechaPathsPtr, 0x53E18A, 7);

                        //
                        process.WriteInstruction(lea, 0x81, hangarmechaInfosPtr + 0x4, 0x53E2A5, 7);
                        process.WriteInstruction(lea, 0x90, hangarmechaInfosPtr + 0x68, 0x53E2D6, 7);
                        process.WriteInstruction(lea, 0x8A, hangarmechaInfosPtr + 0xCC, 0x53E307, 7);
                        process.WriteInstruction(lea, 0x81, hangarmechaInfosPtr + 0x130, 0x53E338, 7);
                        process.WriteInstruction(lea, 0x90, hangarmechaInfosPtr + 0x194, 0x53E369, 7);
                        process.WriteInstruction(lea, 0x8A, hangarmechaInfosPtr + 0x1F8, 0x53E39A, 7);

                        process.WriteInstruction(lea, 0x82, mechaPathsPtr, 0x53E4DC, 7);
                        process.WriteInstruction(otherOtherMov, 0x15, hangarmechaInfosPtr, 0x53E4D3, 6);

                        {
                            process.WriteNops(0x53E613, 12);

                            byte* data = stackalloc byte[5];
                            data[0] = 0xBE;
                            *((uint*)&data[1]) = (uint)mechaInfosPtr;

                            ErrorCheck(process.Write((IntPtr)0x53E613, data, 5));
                        }
                        #endregion

                        {
                            byte* data = stackalloc byte[11];
                            data[0] = mov;
                            data[1] = 0x82;
                            *((uint*)&data[2]) = (uint)mechaInfosPtr + 0x25C;
                            *((uint*)&data[6]) = 0;
                            data[10] = 0x90;

                            ErrorCheck(process.Write((IntPtr)0x518B1C, data, 11));
                        }

                        {
                            byte* data = stackalloc byte[13];
                            data[0] = 0xC7;
                            data[1] = 0x44;
                            data[2] = 0x24;
                            data[3] = 0x04;
                            *((uint*)&data[4]) = numberOfMecha;

                            IntPtr codePtr = ErrorCheck(process.Alloc(13));
                            ErrorCheck(process.Write(codePtr, data, 13));
                            process.WriteJump(codePtr + 8, 0x415DD0);

                            process.WriteCall(0x41A227, codePtr);
                            process.WriteCall(0x4140CD, codePtr);
                        }

                        ErrorCheck(process.Resume());
                    }
                    else
                    {
                        MessageBox.Show("Unable to start WindomXP.exe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
