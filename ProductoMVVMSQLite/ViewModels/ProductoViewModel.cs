using ProductoMVVMSQLite.Models;
using ProductoMVVMSQLite.Utils;
using ProductoMVVMSQLite.Views;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProductoMVVMSQLite.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class ProductoViewModel
    {
        public ObservableCollection<Producto> ListaProductos { get; set; }

        public Producto ProductoSeleccionado {get;set;}

        public ProductoViewModel() {
            Util.ListaProductos = new ObservableCollection<Producto>(App.productoRepository.GetAll());

            ListaProductos = Util.ListaProductos;
        
        }

        public ICommand CrearProducto =>
            new Command(async () =>
            {
               await App.Current.MainPage.Navigation.PushAsync(new NuevoProductoPage());
            });

        

        public ICommand EliminarProductoCommand => 
            new Command<Producto>(async (producto) =>
             {
                bool confirmar = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Esta seguro de eliminar el producto?", "Sí", "No");
                if (confirmar)
                {
                await EliminarProducto(producto);
                }

              });

       //metodo para eliminar
        public async Task EliminarProducto(Producto producto)
        {
            try
            {
                if (producto.IdProducto != 0)
                {
                    App.productoRepository.Delete(producto);
                    ListaProductos.Remove(producto);
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", "El producto no se puede eliminar: " + ex.Message, "Se elimino correctamente");
            }
        }

        public ICommand UpdateCommand => 
            new Command<Producto>(async (producto) =>
        {

            try
            {
                App.productoRepository.Update(producto);

                // Encuentra el producto en la lista y actualiza la información
                var productoLista = ListaProductos.FirstOrDefault(p => p.IdProducto == producto.IdProducto);
                if (productoLista != null)
                {
                    var index = ListaProductos.IndexOf(productoLista);
                    ListaProductos[index] = producto;
                }
                else
                {
                    //Se agrega un producto nuevo
                    ListaProductos.Add(producto);
                }

                // Navega de regreso a la lista de productos o actualiza la vista 
                await App.Current.MainPage.Navigation.PopAsync(); 
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", "No se guardaron los cambios: " + ex.Message, "Ok");
            }
        });






    }
}
