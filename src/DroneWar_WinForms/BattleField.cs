using System;
using System.Drawing;
using System.Windows.Forms;

namespace UPG_SP_2024
{

    /// <summary>
    /// The main panel with the custom visualization
    /// </summary>
    public class BattleField : Panel
    {
        /// <summary>Initializes a new instance of the <see cref="BattleField" /> class.</summary>
        ScenarioData data;

        public BattleField()
        {
            this.ClientSize = new System.Drawing.Size(800, 600);
            data = LoadScenario(0);
            Console.WriteLine(data.W);
        }


        /// <summary>TODO: Custom visualization code comes into this method</summary>
        /// <remarks>Raises the <see cref="E:System.Windows.Forms.Control.Paint">Paint</see> event.</remarks>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs">PaintEventArgs</see> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            //TODO: Add custom paint code here

            g.FillEllipse(Brushes.Red, this.Width / 2, this.Height / 4, 100, 100);

            Font f = new Font("Arial", 24, FontStyle.Bold);

            for (int i = 0; i < data.W; i++)
            {
                for(int j = 0; j < data.H; j++)
                {
                    Console.Write("hello");
                }
            }

            g.DrawString((data.W).ToString(), f, Brushes.Black, 0,0);

            // Calling the base class OnPaint   
            base.OnPaint(e);
        }

        /// <summary>
        /// Fires the event indicating that the panel has been resized. Inheriting controls should use this in favor of actually listening to the event, but should still call <span class="keyword">base.onResize</span> to ensure that the event is fired for external listeners.
        /// </summary>
        /// <param name="eventargs">An <see cref="T:System.EventArgs">EventArgs</see> that contains the event data.</param>
        protected override void OnResize(EventArgs eventargs)
        {
            this.Invalidate();  //ensure repaint

            base.OnResize(eventargs);
        }

      

        private int ReadInt32EndianSafe(BinaryReader br)
        {
            // Přečteme 4 bajty jako pole
            byte[] data = br.ReadBytes(4);

            // Zkontrolujeme, zda systém používá LittleEndian (Windows ano)
            if (BitConverter.IsLittleEndian)
            {
                // je potřeba otočit bity
                Array.Reverse(data);
            }

            // Převod 4 bajtů na int
            return BitConverter.ToInt32(data, 0);
        }

        public ScenarioData LoadScenario(int id)
        {
            // Změna koncovky souboru na .ter
            string filePath = $"D:\\skola\\upg\\semestralni_prace\\dronewar\\data\\0.ter";
            ScenarioData data = new ScenarioData();

            try
            {
                // 1. Otevření binárního souboru pro čtení
                using (FileStream fs = new FileStream(filePath, FileMode.Open))

                // 2. Použití BinaryReader pro čtení primitivních typů
                using (BinaryReader br = new BinaryReader(fs))
                {
                    // ... pokračování čtení ...
                    data.Version = ReadInt32EndianSafe(br);
                    data.W = ReadInt32EndianSafe(br);
                    data.H = ReadInt32EndianSafe(br);
                    data.Dy = ReadInt32EndianSafe(br);
                    data.XS = ReadInt32EndianSafe(br);
                    data.YS = ReadInt32EndianSafe(br);
                    data.AS = ReadInt32EndianSafe(br);
                    data.ZS = ReadInt32EndianSafe(br);
                    data.VS = ReadInt32EndianSafe(br);
                    data.WVx = ReadInt32EndianSafe(br);
                    data.WVy = ReadInt32EndianSafe(br);
                    data.WVz = ReadInt32EndianSafe(br);
                    data.XT = ReadInt32EndianSafe(br);
                    data.YT = ReadInt32EndianSafe(br);

                    // 2. Čtení Matice Výšek Terénu
                    data.TerrainHeights = new int[data.H, data.W];

                    for (int y = 0; y < data.H; y++)
                    {
                        for (int x = 0; x < data.W; x++)
                        {
                            data.TerrainHeights[y, x] = ReadInt32EndianSafe(br);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Došlo k chybě načítání binárního souboru: {ex.Message}", "Chyba");
                return null;
            }
            // ... ošetření chyb ...
            return data;
        }
    }

    public class ScenarioData
    {
        // --- 1. Hlavička Scénáře (15x Int32) ---

        /// <summary>Verze formátu souboru.</summary>
        public int Version { get; set; }

        // Rozměry mapy
        /// <summary>W: Šířka mapy (počet sloupců X).</summary>
        public int W { get; set; }
        /// <summary>H: Výška mapy (počet řádků Y).</summary>
        public int H { get; set; }
        /// <summary>Dx: Rozteč mezi body ve směru X (metry).</summary>
        public int Dx { get; set; }
        /// <summary>Dy: Rozteč mezi body ve směru Y (metry).</summary>
        public int Dy { get; set; }

        // Poloha děla (Start)
        /// <summary>XS: Počáteční poloha děla (sloupec).</summary>
        public int XS { get; set; }
        /// <summary>YS: Počáteční poloha děla (řádek).</summary>
        public int YS { get; set; }

        // Nastavení děla
        /// <summary>AS: Azimut střely (stupně).</summary>
        public int AS { get; set; }
        /// <summary>ZS: Zenit střely (stupně).</summary>
        public int ZS { get; set; }
        /// <summary>VS: Počáteční rychlost střely (m/s).</summary>
        public int VS { get; set; }

        // Vektor větru
        /// <summary>WVx: Složka rychlosti větru ve směru X.</summary>
        public int WVx { get; set; }
        /// <summary>WVy: Složka rychlosti větru ve směru Y.</summary>
        public int WVy { get; set; }
        /// <summary>WVz: Složka rychlosti větru ve směru Z (výška).</summary>
        public int WVz { get; set; }

        // Poloha cíle (Target)
        /// <summary>XT: Poloha cíle (sloupec).</summary>
        public int XT { get; set; }
        /// <summary>YT: Poloha cíle (řádek).</summary>
        public int YT { get; set; }

        // --- 2. Data Terénu (Matice Z_XY) ---

        /// <summary>
        /// Dvojrozměrné pole pro uložení nadmořských výšek terénu.
        /// Indexování je [řádek Y, sloupec X], tj. [H, W].
        /// </summary>
        public int[,] TerrainHeights { get; set; }
    }


}
