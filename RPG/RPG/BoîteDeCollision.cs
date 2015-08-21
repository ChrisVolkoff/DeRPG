using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace AtelierXNA
{
    public class BoîteDeCollision : PrimitiveDeBase
    {
        const int NB_SOMMETS = 10;
        const int NB_TRIANGLES = 8;
        Color Couleur { get; set; }
        VertexPositionColor[] Sommets { get; set; }
        Vector3 Min { get; set; }
        Vector3 Max { get; set; }
        BoundingBox Boîte { get; set; }
        ObjetDeBasePhysique ObjetPhysique { get; set; }
        RasterizerState GestionWireframe { get; set; }
        BasicEffect EffetDeBase { get; set; }
        public ScèneDeJeu ScèneJeu { get; private set; }


        public BoîteDeCollision(RPG jeu, ScèneDeJeu scèneJeu, ObjetDeBasePhysique objetPhysique, BoundingBox boîte, Color couleur)
            : base(jeu)
        {
            ScèneJeu = scèneJeu;
            Boîte = boîte;
            Min = boîte.Min;
            Max = boîte.Max;
            Couleur = couleur;
            ObjetPhysique = objetPhysique;
            Visible = true; // on ne l'affiche pas initialement
        }

        public override void Initialize()
        {
            Monde = ObjetPhysique.GetMonde();
            Sommets = new VertexPositionColor[NB_SOMMETS];
            GestionWireframe = new RasterizerState();
            GestionWireframe.CullMode = CullMode.None;
            GestionWireframe.FillMode = FillMode.WireFrame;
            base.Initialize();
            LoadContent();
        }

        protected override void LoadContent()
        {
            EffetDeBase = new BasicEffect(GraphicsDevice);
            EffetDeBase.VertexColorEnabled = true;
            base.LoadContent();
        }

        protected override void InitialiserSommets()
        {
            Vector3[] listeDesCoins = Boîte.GetCorners();
            Sommets[0] = new VertexPositionColor(listeDesCoins[3], Couleur); //A
            Sommets[1] = new VertexPositionColor(listeDesCoins[0], Couleur);//B
            Sommets[2] = new VertexPositionColor(listeDesCoins[7], Couleur);//C
            Sommets[3] = new VertexPositionColor(listeDesCoins[4], Couleur);//D
            Sommets[4] = new VertexPositionColor(listeDesCoins[6], Couleur);//E
            Sommets[5] = new VertexPositionColor(listeDesCoins[5], Couleur);//F
            Sommets[6] = new VertexPositionColor(listeDesCoins[2], Couleur);//G
            Sommets[7] = new VertexPositionColor(listeDesCoins[1], Couleur);//H
            Sommets[8] = new VertexPositionColor(listeDesCoins[3], Couleur); //A
            Sommets[9] = new VertexPositionColor(listeDesCoins[0], Couleur);//B
        }

        protected void EffectuerMiseÀJour()
        {
            Monde = ObjetPhysique.GetMonde();
        }

        protected void GérerClavier()
        {
            if (ScèneJeu.GestionInput3D.EstNouvelleTouche(Keys.B) &&
               ScèneJeu.GestionInput3D.ÉtatClavier.IsKeyDown(Keys.LeftShift) || ScèneJeu.GestionInput3D.ÉtatClavier.IsKeyDown(Keys.RightShift))
            {
                this.Visible = !this.Visible;
            }
        }

        public void DessinerPrimitive()
        {
            RasterizerState oldRasterizerState = ScèneJeu.GraphicsDevice.RasterizerState;
            ScèneJeu.GraphicsDevice.RasterizerState = GestionWireframe;
            EffetDeBase.World = GetMonde();
            EffetDeBase.View = ScèneJeu.CaméraJeu.Vue;
            EffetDeBase.Projection = ScèneJeu.CaméraJeu.Projection;
            foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
            {
                passeEffet.Apply();
                ScèneJeu.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, Sommets, 0, NB_TRIANGLES);
            }

            ScèneJeu.GraphicsDevice.RasterizerState = oldRasterizerState;
        }
    }
}
