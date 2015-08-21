using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace AtelierXNA
{
    public class Scène : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public GestionnaireDeScènes SceneManager { get; private set; }
        public SpriteBatch GestionSprites { get; private set; }
        public RessourcesManager<SpriteFont> GestionnaireDeFonts { get; private set; }
        public RessourcesManager<Texture2D> GestionnaireDeTextures { get; private set; }
        public RessourcesManager<Model> GestionnaireDeModèles { get; private set; }
        public InputManager GestionInput { get; private set; }
        public Random GénérateurAléatoire { get; set; }

        public RPG Jeu { get; set; }
        protected List<GameComponent> ListeDesÉléments { get; set; }

        public Scène(RPG jeu, GestionnaireDeScènes scènesMgr)
            : base(jeu)
        {
            Jeu = jeu;
            SceneManager = scènesMgr;

            ListeDesÉléments = new List<GameComponent>();

            GetServices();
        }

        // Appelée lorsque la scène est activée (Initialize, mais à chaque activation de la scène)
        public virtual void Activate() { }

        protected virtual void GérerClavier() { }

        public override void Initialize()
        {
            foreach (GameComponent élément in ListeDesÉléments)
            {
                élément.Initialize();
            }
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < ListeDesÉléments.Count; ++i)
            {
               if (ListeDesÉléments[i].Enabled)
                  ListeDesÉléments[i].Update(gameTime);
            }
            base.Update(gameTime);
        }
        
        public override void Draw(GameTime gameTime)
        {
            foreach (GameComponent élément in ListeDesÉléments)
            {
                if (élément is DrawableGameComponent)
                {
                    if (((DrawableGameComponent)élément).Visible)
                    ((DrawableGameComponent)élément).Draw(gameTime);
                }
            }
            base.Draw(gameTime);
        }

        private void GetServices()
        {
            GestionSprites = (SpriteBatch)Jeu.Services.GetService(typeof(SpriteBatch));
            GestionnaireDeFonts = (RessourcesManager<SpriteFont>)Jeu.Services.GetService(typeof(RessourcesManager<SpriteFont>));
            GestionnaireDeTextures = (RessourcesManager<Texture2D>)Jeu.Services.GetService(typeof(RessourcesManager<Texture2D>));
            GestionnaireDeModèles = (RessourcesManager<Model>)Jeu.Services.GetService(typeof(RessourcesManager<Model>));
            GestionInput = (InputManager)Jeu.Services.GetService(typeof(InputManager));
            GénérateurAléatoire = (Random)Jeu.Services.GetService(typeof(Random));
        }
    }
}