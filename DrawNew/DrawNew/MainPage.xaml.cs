using System;
using System.Collections.Generic;

using Xamarin.Forms;

using TouchTracking;
using TouchTracking.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace DrawNew
{
    public partial class MainPage : ContentPage
    {
        //slovník pro započatou cestu, která pokračuje dokuď se neuvolní dotyk
        Dictionary<long, SKPath> inProgressPaths = new Dictionary<long, SKPath>();
        //List, který ukládá všechny zakreslené cesty
        List<SKPath> completedPaths = new List<SKPath>();
        //List, který funguje jako list předchozí, měl vyřešit problém při měnení barvy, ovšem neúspěšně
        List<SKPath> completedPaths2 = new List<SKPath>();

        //Pole knihovny SkiaSharp, který uchovává základní styly a parametry kreslení
        SKPaint paint = new SKPaint
        {
            //Definuje styl při kterém se bude kreslit > Tahem
            Style = SKPaintStyle.Stroke,
            //Definuje Barvu, defaultně černá
            Color = SKColors.Red,
            //Definuje šířku tahu, respektive výsledné čáry
            StrokeWidth = 10,
            
        };

        //Definuje Bitmapu
        public SKBitmap saveBitmap;

        public MainPage()
        {
            InitializeComponent();
        }

        //Třída umožňující zobrazení kreseb na canvas
        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            //Jednotlivé informace o vzniklém obrazu/kreslící ploše > následně lze využít pro uložení do galerie
            SKImageInfo info = args.Info;
            //Informace o povrchu Canvasu
            SKSurface surface = args.Surface;
            //Veškeré informace a stav canvasu zabaluje do jedné proměnné
            SKCanvas canvas = surface.Canvas;

            //Pokud není žádná bitmapa k dispozici vytvoř novou
            if (saveBitmap == null)
            {
                saveBitmap = new SKBitmap(info.Width, info.Height);
            }
            //Vzniklou či aktuální bitmapu zkontroluje, zda sedí s rozměry z info, vygeneruje novou bitmapu a´té nastaví tyto parametry, tu následně přepíše přes původní
            else if (saveBitmap.Width < info.Width || saveBitmap.Height < info.Height)
            {
                SKBitmap newBitmap = new SKBitmap(Math.Max(saveBitmap.Width, info.Width),
                                                  Math.Max(saveBitmap.Height, info.Height));

                using (SKCanvas newCanvas = new SKCanvas(newBitmap))
                {
                    newCanvas.Clear();
                    newCanvas.DrawBitmap(saveBitmap, 0, 0);
                }

                saveBitmap = newBitmap;
            }

            //Vyčistí canvas od přředchozí bitmapy
            canvas.Clear();
            canvas.DrawBitmap(saveBitmap, 0, 0);
        }

        //Tvoří jednotlivé akce podle stavu dotyku displeje pomocí knihovny TouchTracking
        void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            //Switch podle typu dotyku
            switch (args.Type)
            {
                //pokud stisknuto, Zkontroluje id křivky, vytvoří novou proměnóu SKPath(vektor), převede její počátek na lokaci v pixelech a přidává do započatých cest a updatuje bitmapu
                case TouchActionType.Pressed:
                    if (!inProgressPaths.ContainsKey(args.Id))
                    {
                        SKPath path = new SKPath();
                        path.MoveTo(ConvertToPixelUsingPoint(new Point { X = args.Location.X, Y = args.Location.Y }));
                        inProgressPaths.Add(args.Id, path);
                        UpdateBitmap();
                    }
                    break;
                //pokud pohnuto dotykem, zkontroluje id, podle toho si jí přebere a naváže na poslední pixel, kde je zaznamenána a pokračuje v přepisování bodů, dokud se nezmění stav dotyku a updatuje bitmapu
                case TouchActionType.Moved:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        SKPath path = inProgressPaths[args.Id];
                        path.LineTo(ConvertToPixelUsingPoint(new Point { X = args.Location.X, Y = args.Location.Y }));
                        UpdateBitmap();
                    }
                    break;
                //pokud dotyk skončí, tak přidá do listu kompletních cest a odebere ze slovníku rozdělané cesty a updatuje bitmapu 
                case TouchActionType.Released:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        completedPaths.Add(inProgressPaths[args.Id]);                        
                        inProgressPaths.Remove(args.Id);
                        UpdateBitmap();
                    }
                    break;
                //Při přerušení dotyku jinou akci se cesta nikam neuloží, pouze vymaže ze slovníku rozdělané cesty
                case TouchActionType.Cancelled:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        inProgressPaths.Remove(args.Id);
                        UpdateBitmap();
                    }
                    break;
            }
        }        

        //Převedení na pixel, při využití pointu
        SKPoint ConvertToPixelUsingPoint(Point pt)
        {
            return new SKPoint((float)(canvasView.CanvasSize.Width * pt.X / (canvasView.Width)),
                           (float)(canvasView.CanvasSize.Height * pt.Y / (canvasView.Height)));
        }

        //Metoda pro obnovení bitmapy uložení do canvasu
        void UpdateBitmap()
        {
            //vezme předchozí mapu, převede do nové a tu celou překreslí
            using (SKCanvas saveBitmapCanvas = new SKCanvas(saveBitmap))
            {
                saveBitmapCanvas.Clear();

                

                foreach (SKPath path in inProgressPaths.Values)
                {
                    saveBitmapCanvas.DrawPath(path, paint);
                }

                foreach (SKPath path in completedPaths2)
                {
                    saveBitmapCanvas.DrawPath(path, paint);
                }
                
            }

            canvasView.InvalidateSurface();
        }
        //tlačítko pro vyčištění aktuální bitmapy
        void OnClearButtonClicked(object sender, EventArgs args)
        {
            completedPaths2.Clear();
            inProgressPaths.Clear();
            UpdateBitmap();
            canvasView.InvalidateSurface();
        }
        //tlačítko pro změnu na červenou barvu
        private void OnRedButtonClicked(object sender, EventArgs e)
        {
            completedPaths2 = completedPaths;
            //completedPaths.Clear();
            paint.Color = SKColors.Red;
            UpdateBitmap();
        }
        //tlačítko pro změnu na modrou barvu
        private void OnBlueButtonClicked(object sender, EventArgs e)
        {
            completedPaths2 = completedPaths;
            //completedPaths.Clear();
            paint.Color = SKColors.Blue;
            UpdateBitmap();
        }
        //tlačítko pro změnu na větší šíři tahy
        private void OnBiggerButtonClicked(object sender, EventArgs e)
        {
            completedPaths2 = completedPaths;
            //completedPaths.Clear();
            inProgressPaths.Clear();
            paint.StrokeWidth = 30;
            UpdateBitmap();

        }
        //tlačítko pro uložení do galerie, nefunkční kod, proto zakomentovaný, aby nepadala aplikace
        async void OnSaveButtonClicked(object sender, EventArgs args)
        {
                /*using (SKImage image = SKImage.FromBitmap(saveBitmap))
                {
                    SKData data = image.Encode();
                    DateTime dt = DateTime.Now;
                    string filename = String.Format("FingerPaint-{0:D4}{1:D2}{2:D2}-{3:D2}{4:D2}{5:D2}{6:D3}.png",
                                                    dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);

                    IPhotoLibrary photoLibrary = DependencyService.Get<IPhotoLibrary>();
                    bool result = await photoLibrary.SavePhotoAsync(data.ToArray(), "FingerPaint", filename);

                    if (!result)
                    {
                        await DisplayAlert("FingerPaint", "Artwork could not be saved. Sorry!", "OK");
                    }
                }*/
        }
        //tlačítko pro vybrání z galerie, také nefunkční
        private void OnPickButtonClicked(object sender, EventArgs e)
        {
            //nedaří se zprovoznit přístup do galerie

        }



    }

    
}

