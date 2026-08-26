using CafeMissionario.Data;
using CafeMissionario.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Globalization;

namespace CafeMissionario.ViewModels
{
    [QueryProperty(nameof(ProdutoParaEditar), "ProdutoParaEditar")]
    public partial class ProdutoCadViewModel : BaseViewModel
    {
        // Propriedades
        [ObservableProperty] private int _id;
        [ObservableProperty] private string _nome = string.Empty;
        [ObservableProperty] private string _preco = string.Empty;
        [ObservableProperty] private string _quantidade = string.Empty;
        [ObservableProperty] private bool _controlaEstoque = true;
        [ObservableProperty] private Produto _produtoParaEditar;

        // --- Propriedades da ficha técnica ---
        [ObservableProperty] private ObservableCollection<Produto> _insumosDisponiveis = new();
        [ObservableProperty] private Produto _insumoSelecionado;
        [ObservableProperty] private string _qtdIngredienteTexto = "1";
        [ObservableProperty] private ObservableCollection<IngredienteItem> _listaIngredientes = new();

        // Construtor
        public ProdutoCadViewModel()
        {
            CarregarInsumos();
        }

        // Métodos
        partial void OnProdutoParaEditarChanged(Produto value)
        {
            if (value != null)
            {
                Id = value.Id;
                Nome = value.Nome;
                Preco = value.Preco.ToString("F2");
                ControlaEstoque = value.ControlaEstoque;
                Quantidade = value.QuantidadeEstoque.ToString();
            }

            // Carrega a ficha técnica cadastrada para este produto
            using var db = new AppDbContext();
            var receita = db.FichasTecnicas
                            .Where(f => f.ProdutoId == value.Id)
                            .ToList();

            ListaIngredientes.Clear();
            foreach (var item in receita)
            {
                var insumo = db.Produtos.Find(item.InsumoId);
                if (insumo != null)
                {
                    ListaIngredientes.Add(new IngredienteItem
                    {
                        InsumoId = insumo.Id,
                        NomeInsumo = insumo.Nome,
                        QuantidadeConsumida = item.QuantidadeConsumida
                    });
                }
            }
        }

        private void CarregarInsumos()
        {
            using var db = new AppDbContext();
            // Carrega todos os produtos para poderem ser escolhidos como ingredientes
            InsumosDisponiveis = new ObservableCollection<Produto>(db.Produtos.ToList());
        }

        // Comandos
        [RelayCommand]
        private async Task SalvarProduto()
        {
            if (string.IsNullOrWhiteSpace(Nome))
            {
                await Shell.Current.DisplayAlertAsync("Aviso", "Preencha o nome do produto.", "OK");
                return;
            }

            string precoAjustado = Preco.Replace(",", ".").Trim();
            if (!decimal.TryParse(precoAjustado,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal precoConvertido))
            {
                await Shell.Current.DisplayAlertAsync("Aviso", "Preço inválido.", "OK");
                return;
            }

            decimal.TryParse(Quantidade.Replace(",", ".").Trim(),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal qtdEstoqueConvertida);

            using var db = new AppDbContext();
            Produto produtoSalvo;

            if (Id > 0)
            {
                produtoSalvo = db.Produtos.Find(Id);
                if (produtoSalvo != null)
                {
                    produtoSalvo.Nome = Nome;
                    produtoSalvo.Preco = precoConvertido;
                    produtoSalvo.ControlaEstoque = ControlaEstoque;
                    produtoSalvo.QuantidadeEstoque = qtdEstoqueConvertida;
                    db.Produtos.Update(produtoSalvo);
                }
            }
            else
            {
                produtoSalvo = new Produto
                {
                    Nome = Nome,
                    Preco = precoConvertido,
                    ControlaEstoque = ControlaEstoque,
                    QuantidadeEstoque = qtdEstoqueConvertida
                };
                db.Produtos.Add(produtoSalvo);
            }

            await db.SaveChangesAsync();

            // GRAVAÇÃO DA FICHA TÉCNICA
            // 1. Remove receitas antigas deste produto (se for edição)
            var antigas = db.FichasTecnicas.Where(f => f.ProdutoId == produtoSalvo.Id);
            db.FichasTecnicas.RemoveRange(antigas);

            // 2. Insere a nova lista de ingredientes
            foreach (var ing in ListaIngredientes)
            {
                db.FichasTecnicas.Add(new FichaTecnica
                {
                    ProdutoId = produtoSalvo.Id,
                    InsumoId = ing.InsumoId,
                    QuantidadeConsumida = ing.QuantidadeConsumida
                });
            }

            await db.SaveChangesAsync();

            await Shell.Current.DisplayAlertAsync("Sucesso", "Produto e Ficha Técnica salvos com sucesso!", "OK");
            await Shell.Current.GoToAsync("..");

            // Limpa os campos para o próximo cadastro
            Nome = string.Empty;
            Preco = string.Empty;
            Quantidade = string.Empty;
            ControlaEstoque = true;
        }





        [RelayCommand]
        private void AdicionarIngrediente()
        {
            if (InsumoSelecionado == null) return;

            string qtdFormatada = QtdIngredienteTexto.Replace(",", ".").Trim();
            if (!decimal.TryParse(qtdFormatada, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal qtdConvertida) || qtdConvertida <= 0)
            {
                Shell.Current.DisplayAlertAsync("Aviso", "Informe uma quantidade válida para o ingrediente.", "OK");
                return;
            }

            // Verifica se o ingrediente já está na lista
            var existente = ListaIngredientes.FirstOrDefault(i => i.InsumoId == InsumoSelecionado.Id);
            if (existente != null)
            {
                existente.QuantidadeConsumida += qtdConvertida;
            }
            else
            {
                ListaIngredientes.Add(new IngredienteItem
                {
                    InsumoId = InsumoSelecionado.Id,
                    NomeInsumo = InsumoSelecionado.Nome,
                    QuantidadeConsumida = qtdConvertida
                });
            }

            QtdIngredienteTexto = "1";
        }

        // COMANDO: Remover Ingrediente da Lista
        [RelayCommand]
        private void RemoverIngrediente(IngredienteItem item)
        {
            if (item != null && ListaIngredientes.Contains(item))
            {
                ListaIngredientes.Remove(item);
            }
        }
    }
}
