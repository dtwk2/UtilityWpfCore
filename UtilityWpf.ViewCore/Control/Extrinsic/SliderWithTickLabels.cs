using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UtilityWpf.View.Extrinsic
{
    /// <summary>
    /// Siga os passos 1a ou 1b e depois 2 para usar esse controle personalizado em um arquivo XAML.
    ///
    /// Passo 1a) Usando o controle personalizado em um arquivo XAML que já existe no projeto atual.
    /// Adicione o atributo XmlNamespace ao elemento raiz do arquivo de marcação onde ele
    /// deve ser usado:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SliderWithTickLabels"
    ///
    ///
    /// Passo 1b) Usando o controle personalizado em um arquivo XAML que existe em um projeto diferente.
    /// Adicione o atributo XmlNamespace ao elemento raiz do arquivo de marcação onde ele
    /// deve ser usado:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SliderWithTickLabels;assembly=SliderWithTickLabels"
    ///
    /// Também será necessário adicionar nesse projeto uma referência ao projeto que contém esse arquivo XAML
    /// e Recompilar para evitar erros de compilação:
    ///
    ///     No Gerenciador de Soluções, clique com o botão direito no projeto alvo e
    ///     "Adicionar Referência"->"Projetos"->[Selecione esse projeto]
    ///
    ///
    /// Passo 2)
    /// Vá em frente e use seu controle no arquivo XAML.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    public class SliderWithTickLabels : Slider
    {
        public static readonly DependencyProperty GeneratedTicksProperty;
        public static readonly DependencyProperty TickLabelTemplateProperty;
        private object sync = new object();

        [Bindable(true)]
        public DoubleCollection GeneratedTicks
        {
            get => base.GetValue(SliderWithTickLabels.GeneratedTicksProperty) as DoubleCollection;
            set => base.SetValue(SliderWithTickLabels.GeneratedTicksProperty, value);
        }

        [Bindable(true)]
        public DataTemplate TickLabelTemplate
        {
            get => base.GetValue(SliderWithTickLabels.TickLabelTemplateProperty) as DataTemplate;
            set => base.SetValue(SliderWithTickLabels.TickLabelTemplateProperty, value);
        }

        static SliderWithTickLabels()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SliderWithTickLabels), new FrameworkPropertyMetadata(typeof(SliderWithTickLabels)));

            SliderWithTickLabels.GeneratedTicksProperty = DependencyProperty.Register("GeneratedTicks", typeof(DoubleCollection), typeof(SliderWithTickLabels), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
            SliderWithTickLabels.TickLabelTemplateProperty = DependencyProperty.Register("TickLabelTemplate", typeof(DataTemplate), typeof(SliderWithTickLabels), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        }

        public SliderWithTickLabels()
        {
            //Uri resourceLocater = new Uri("/UtilityWpf.ViewCore;component/themes/Extrinsic/SliderWithTickLabels.xaml", System.UriKind.Relative);
            //ResourceDictionary resourceDictionary = (ResourceDictionary)Application.LoadComponent(resourceLocater);
            //Style = resourceDictionary["SliderWithTickLabels"] as Style;

            CalculateTicks();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            var propertyNames = new string[] { "Minimum", "Maximum", "TickFrequency", "Ticks", "IsDirectionReversed" };

            if (IsInitialized && propertyNames.Contains(e.Property.Name))
            {
                CalculateTicks();
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.CalculateTicks();
        }

        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            return ClipToBounds ? base.GetLayoutClip(layoutSlotSize) : null;
        }

        private async void CalculateTicks()
        {
            double[] ticks = null;
            double min;
            double max;
            double tickFrequency;
            lock (sync)
            {
                ticks = Ticks.Select(_ => _).ToArray();
                min = Minimum;
                max = Maximum;
                tickFrequency = this.TickFrequency;
            }

            await Task.Run(async () =>
        {
            DoubleCollection c = null;
            try
            {
                if (ticks != null && ticks.Any())
                {
                    await this.Dispatcher.InvokeAsync(() =>
                    {
                        this.GeneratedTicks = new DoubleCollection(ticks.Union(new double[] { min, max }).Where(t => min <= t && t <= max));
                    }, System.Windows.Threading.DispatcherPriority.Background);
                }
                else if (tickFrequency > 0.0)
                {
                    long l = (long)Math.Ceiling((max - min) / tickFrequency) + 1;
                    double[] range = Enumerable.Range(0, (int)l).Select(t => Math.Min(t * tickFrequency + min, max)).ToArray();
                    await this.Dispatcher.InvokeAsync(() =>
                    {
                        if (l <= int.MaxValue)
                        {
                            this.GeneratedTicks = new DoubleCollection(range);
                        }
                        //else
                        //{
                        //    this.GeneratedTicks = new DoubleCollection(Enumerable.Range(0, int.MaxValue).Select(t => Math.Min(t * tickFrequency + min, max)));
                        //}
                    }, System.Windows.Threading.DispatcherPriority.Background);
                }
            }
            catch (Exception ex)
            {
                await this.Dispatcher.InvokeAsync(() => MessageBox.Show(ex.Message));
            }
        });
        }
    }
}