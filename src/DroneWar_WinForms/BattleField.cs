using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;


namespace UPG_SP_2024
{

    /// <summary>
    /// The main panel with the custom visualization
    /// </summary>
    public class BattleField : Panel
    {
        /// <summary>Initializes a new instance of the <see cref="BattleField" /> class.</summary>
        ScenarioData data;
        int min = 0, max = 0;
        Bitmap terrainBitmap;

        int panelWidth;
        int panelHeight;

        //where to move the terrain
        int x = 0;
        int y = 0;
        float scale = 1.0f;

        Drone drone;


        private readonly System.Windows.Forms.Timer simulationTimer;
        private const double TimeStep = 0.01; // Časový krok Delta T = 10 ms

        public BattleField()
        {
            this.ClientSize = new System.Drawing.Size(800, 600);

            simulationTimer = new System.Windows.Forms.Timer();
            simulationTimer.Interval = (int)(TimeStep * 1000); // 10 milisekund
            simulationTimer.Tick += SimulationTimer_Tick; // Připojení obslužné metody

            data = LoadScenario(0);

            for (int i = 0;i < data.W; i++)
            {
                for (int j = 0; j < data.H; j++)
                {
                    if(data.TerrainHeights[j,i] < min) min = data.TerrainHeights[j,i];
                    if(data.TerrainHeights[j,i] > max) max = data.TerrainHeights[j,i];
                }
            }

            GenerateTerrainBitmap();

            panelWidth = this.ClientSize.Width;
            panelHeight = this.ClientSize.Height;

            drone = new Drone(data.XT, data.YT);
        }


        /// <summary>TODO: Custom visualization code comes into this method</summary>
        /// <remarks>Raises the <see cref="E:System.Windows.Forms.Control.Paint">Paint</see> event.</remarks>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs">PaintEventArgs</see> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {

            if (data == null || terrainBitmap == null) return;
            Graphics g = e.Graphics;

            panelWidth = this.ClientSize.Width;
            panelHeight = this.ClientSize.Height;

            float scaleX = (float)panelWidth / data.W;
            float scaleY = (float)panelHeight / data.H;
            scale = (float)Math.Max(scaleX, scaleY);

            //TODO: Add custom paint code here

            Font f = new Font("Arial", 24, FontStyle.Bold);

            g.DrawImage(terrainBitmap, 0,0,(float)data.W*scale, (float)data.H * scale);

            g.DrawString((data.XT).ToString(), f, Brushes.Black, 0,0);
            g.DrawString((data.YT).ToString(), f, Brushes.Black, 0, 50);

            Rectangle canon = new Rectangle((int)(data.XS * scale)-10, (int)(data.YS * scale)-10, 20, 20);
            

            g.FillEllipse(Brushes.Black, canon);

            drone.drawDrone(g);

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

        private void SimulationTimer_Tick(object sender, EventArgs e)
        {
            if (currentProjectileState == null) return;

            // 2. Provedeme jeden simulační krok
            // ZDE BUDE: currentProjectileState = IntegrateRK4(currentProjectileState, TimeStep);

            // Dočasná ukázka pohybu (pouze gravitace)
            currentProjectileState.Vz -= 9.81 * TimeStep; // Změna rychlosti
            currentProjectileState.Z += currentProjectileState.Vz * TimeStep; // Změna pozice
            currentProjectileState.Time += TimeStep;

            // 3. Kontrola zastavení (např. dopad na zem)
            if (currentProjectileState.Z < 0)
            {
                simulationTimer.Stop();
                currentProjectileState.Z = 0;
            }

            // 4. Překreslení scény s novou pozicí
            this.Invalidate();
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

        private Color GetTerrainColor(int altitude)
        {
            if (max == min) return Color.Gray;

            // 1. Normalizace výšky do rozsahu 0.0 až 1.0
            double normalized = (double)(altitude - min) / (max - min);

            // 2. Mapování do barevného gradientu (Zelená -> Hnědá -> Bílá)

            if (normalized < 0.5)
            {
                // Spodní polovina (Nízké výšky): Zelená k Žluté/Hnědé
                int r = (int)(normalized * 2 * 255); // Od 0 do ~128
                int g = (int)(255 - normalized * 2 * 100); // Od 255 do ~55
                int b = (int)(normalized * 2 * 255 * 0.1); // Od 0 do ~25
                return Color.FromArgb(r, g, b);
            }
            else
            {
                // Horní polovina (Vysoké výšky): Žlutá/Hnědá k Bílé
                double highNormalized = (normalized - 0.5) * 2; // Rozsah 0.0 až 1.0 pro vysokou část
                int component = (int)(128 + highNormalized * 127); // Od 128 do 255
                return Color.FromArgb(component, component, component);
            }
        }


        // --- NOVÁ METODA: Vytvoření Mapy do Bitmapy ---
        private void GenerateTerrainBitmap()
        {
            // Ujistíme se, že máme data a můžeme pracovat s rozměry
            if (data == null) return;

            int W = data.W; // Šířka mapy v buňkách/pixelech
            int H = data.H; // Výška mapy v buňkách/pixelech

            // Krok A: Uvolnění staré bitmapy, pokud existuje
            if (terrainBitmap != null)
            {
                terrainBitmap.Dispose();
            }

            // Krok B: Vytvoření nové instance Bitmapy
            // Rozměry Bitmapy budou přesně shodné s rozměry datové matice
            terrainBitmap = new Bitmap(W, H);

            // Nyní můžete pokračovat nastavením pixelů pomocí for cyklů a metody SetPixel.

            for (int y = 0; y < H; y++)
            {
                for (int x = 0; x < W; x++)
                {
                    int altitude = data.TerrainHeights[y, x];
                    Color terrainColor = GetTerrainColor(altitude);

                    // Krok C: Nastavení barvy pixelu (x, y)
                    terrainBitmap.SetPixel(x, y, terrainColor);
                }
            }
        }

        public ScenarioData LoadScenario(int id)
        {
            // Změna koncovky souboru na .ter
            string filePath = $"D:\\skola\\upg\\semestralni_prace\\dronewar\\data\\2.ter";
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

    public class Drone
    {
        int x;
        int y;

        public Drone(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void Move(int deltaX, int deltaY)
        {
            x += deltaX;
            y += deltaY;
        }

        public void drawDrone(Graphics g)
        {
            Rectangle drone = new Rectangle(x - 10, y - 10, 20, 20);
            g.FillRectangle(Brushes.Red, drone);
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

    public class ProjectileState
    {
        // Pozice (metrů)
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        // Rychlost (metry za sekundu)
        public double Vx { get; set; }
        public double Vy { get; set; }
        public double Vz { get; set; }

        // Celkový čas simulace od startu
        public double Time { get; set; }

        /// <summary>
        /// Inicializace stavu.
        /// </summary>
        public ProjectileState(double x, double y, double z, double vx, double vy, double vz, double time = 0.0)
        {
            X = x;
            Y = y;
            Z = z;
            Vx = vx;
            Vy = vy;
            Vz = vz;
            Time = time;
        }
    }
}
