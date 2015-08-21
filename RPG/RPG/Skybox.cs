using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{
    public class Skybox : ObjetDeBase
    {
        public Vector3 PositionOffset { get; set; }
        DepthStencilState ZBufferTemporaire { get; set; }
        DepthStencilState ZBufferBackUp { get; set; }

        Texture2D[] Textures { get; set; }
        string[] NomTextures { get; set; }
        
        public Skybox(RPG jeu, ScèneDeJeu scèneJeu, string nomModèle, string[] nomtextures, Vector3 positionOffset)
            : base(jeu, scèneJeu, nomModèle, 1f, Vector3.Zero, Vector3.Zero)
        {
           NomTextures = nomtextures;
            PositionOffset = positionOffset;
        }

        public override void Initialize()
        {
            ZBufferTemporaire = new DepthStencilState();
            ZBufferTemporaire.DepthBufferEnable = false;
            base.Initialize();
        }

        protected override void LoadContent()
        {
           Textures = new Texture2D[NomTextures.Length];
           for (int i = 0; i < NomTextures.Length; ++i)
           {
              Textures[i] = ScèneJeu.GestionnaireDeTextures.Find(NomTextures[i]);
           }
           base.LoadContent();
        }
        public override void Draw(GameTime gameTime)
        {
            ZBufferBackUp = Jeu.PériphériqueGraphique.GraphicsDevice.DepthStencilState;
            Jeu.PériphériqueGraphique.GraphicsDevice.DepthStencilState = ZBufferTemporaire;
            int i = 0;
            foreach (ModelMesh maille in Modèle.Meshes)
            {
                Matrix mondeLocal = TransformationsModèle[maille.ParentBone.Index] * GetMonde();
                foreach (ModelMeshPart portionDeMaillage in maille.MeshParts)
                {
                    BasicEffect effet = (BasicEffect)portionDeMaillage.Effect;
                    effet.LightingEnabled = false;
                    effet.Projection = ScèneJeu.CaméraJeu.Projection;
                    effet.View = ScèneJeu.CaméraJeu.Vue;
                    effet.World = mondeLocal;
                    effet.Texture = Textures[i++];
                }
                maille.Draw();
            }

            Jeu.PériphériqueGraphique.GraphicsDevice.DepthStencilState = ZBufferBackUp;
        }

        public override void Update(GameTime gameTime)
        {
            if (Position != ScèneJeu.CaméraJeu.Position)
            {
                Position = ScèneJeu.CaméraJeu.Position;
                Monde = Matrix.Identity;
                Monde *= Matrix.CreateTranslation(Position - PositionOffset);
            }
            base.Update(gameTime);
        }
    }
}
