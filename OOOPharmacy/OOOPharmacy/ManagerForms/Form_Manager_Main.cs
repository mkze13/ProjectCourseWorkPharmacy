using OOOPharmacy.ManagerForms.ManagerFormClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOOPharmacy.ManagerForms
{
    public partial class Form_Manager_Main : Form
    {

        DataBase dataBase = new DataBase();
        TableEmployees tableEmployees = new TableEmployees();
        TableClients tableClients = new TableClients();
        TableProducts tableProducts = new TableProducts();
        TableHistoryCost tableHistoryCost = new TableHistoryCost();
        TableBranchesProducts tableBranchesProducts = new TableBranchesProducts();
        TableBranches tableBranches = new TableBranches();
        TableOrders tableOrders = new TableOrders();
        TableProductsInOrders tableProductsInOrders = new TableProductsInOrders();
        TableSuppliers tableSuppliers = new TableSuppliers();
        TableSales tableSales = new TableSales();

        int rowIndex;

        public Form_Manager_Main()
        {
            InitializeComponent();
        }

        private void таблицаСотрудникиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tableEmployees.RefreshDataGridView(dataGridViewEmployees);

            tabControlEmployees.Visible = true;
            tabControlClients.Visible = false;
            tabControlProducts.Visible = false;
            tabControlBranches.Visible = false;
            tabControlOrders.Visible = false;
            tabControlOrders.Visible = false;
        }

        private void клиентыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tableClients.RefreshDataGridView(dataGridViewClients);

            tabControlClients.Visible = true;
            tabControlEmployees.Visible = false;
            tabControlProducts.Visible = false;
            tabControlBranches.Visible = false;
            tabControlOrders.Visible = false;
            tabControlOrders.Visible = false;
        }

        private void товарToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tableProducts.RefreshDataGridView(dataGridViewProducts);
            tableHistoryCost.RefreshDataGridView(dataGridViewHistoryCost);
            tableBranchesProducts.RefreshDataGridView(dataGridViewBranchesProducts);

            tabControlProducts.Visible = true;
            tabControlClients.Visible = false;
            tabControlEmployees.Visible = false;
            tabControlBranches.Visible = false;
            tabControlOrders.Visible = false;
            tabControlOrders.Visible = false;
        }

        private void магазиныToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tableBranches.RefreshDataGridView(dataGridViewBranches);

            tabControlBranches.Visible = true;
            tabControlProducts.Visible = false;
            tabControlClients.Visible = false;
            tabControlEmployees.Visible = false;
            tabControlOrders.Visible = false;
            tabControlOrders.Visible = false;
        }

        private void заказыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tableOrders.RefreshDataGridView(dataGridViewOrders);
            tableProductsInOrders.RefreshDataGridView(dataGridViewProductsInOrders);
            tableSuppliers.RefreshDataGridView(dataGridViewSuppliers);

            tabControlOrders.Visible = true;
            tabControlProducts.Visible = false;
            tabControlClients.Visible = false;
            tabControlEmployees.Visible = false;
            tabControlBranches.Visible = false;
            tabControlSales.Visible = false;
        }

        private void продажиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tableSales.RefreshDataGridView(dataGridViewSales);

            tabControlSales.Visible = true;
            tabControlOrders.Visible = false;
            tabControlProducts.Visible = false;
            tabControlClients.Visible = false;
            tabControlEmployees.Visible = false;
            tabControlBranches.Visible = false;
        }

        private void Form_Manager_Main_Load(object sender, EventArgs e)
        {
            try
            {
                tableEmployees.CreateColumns(dataGridViewEmployees);
                tableClients.CreateColumns(dataGridViewClients);
                tableProducts.CreateColumns(dataGridViewProducts);
                tableHistoryCost.CreateColumns(dataGridViewHistoryCost);
                tableBranchesProducts.CreateColumns(dataGridViewBranchesProducts);
                tableBranches.CreateColumns(dataGridViewBranches);
                tableOrders.CreateColumns(dataGridViewOrders);
                tableProductsInOrders.CreateColumns(dataGridViewProductsInOrders);
                tableSuppliers.CreateColumns(dataGridViewSuppliers);
                tableSales.CreateColumns(dataGridViewSales);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }
        }

        private void Form_Manager_Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void buttonUpdateEmployees_Click(object sender, EventArgs e)
        {
            tableEmployees.RefreshDataGridView(dataGridViewEmployees);
        }

        private void buttonSaveEmployees_Click(object sender, EventArgs e)
        {
            tableEmployees.parametersForEmployees(dataGridViewEmployees);
        }

        private void textBoxSearchEmployees_TextChanged(object sender, EventArgs e)
        {
            tableEmployees.SearchEmployees(dataGridViewEmployees, textBoxSearchEmployees);
        }

        private void buttonAddEmployees_Click(object sender, EventArgs e)
        {
            rowIndex = dataGridViewEmployees.Rows.Add();

            DataGridViewRow newRow = dataGridViewEmployees.Rows[rowIndex];

            dataGridViewEmployees.CurrentCell = newRow.Cells["Last_name"];

            dataGridViewEmployees.BeginEdit(true);
        }

        private void buttonDeleteEmployees_Click(object sender, EventArgs e)
        {
            tableEmployees.DeleteEmployees(dataGridViewEmployees);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tableClients.RefreshDataGridView(dataGridViewClients);
        }

        private void buttonDeleteClients_Click(object sender, EventArgs e)
        {
            tableClients.DeleteClients(dataGridViewClients);
        }

        private void buttonSaveClients_Click(object sender, EventArgs e)
        {
            tableClients.parametersForClients(dataGridViewClients);
        }

        private void textBoxSearchClients_TextChanged(object sender, EventArgs e)
        {
            tableClients.SearchClients(dataGridViewClients, textBoxSearchClients);
        }

        private void buttonRefreshProducts_Click(object sender, EventArgs e)
        {
            tableProducts.RefreshDataGridView(dataGridViewProducts);
        }

        private void buttonDeleteProducts_Click(object sender, EventArgs e)
        {
            tableProducts.DeleteProduct(dataGridViewProducts);
        }

        private void buttonSaveProducts_Click(object sender, EventArgs e)
        {
            tableProducts.parametersForProducts(dataGridViewProducts);
        }

        private void buttonAddProducts_Click(object sender, EventArgs e)
        {
            rowIndex = dataGridViewProducts.Rows.Add();

            DataGridViewRow newRow = dataGridViewProducts.Rows[rowIndex];

            dataGridViewProducts.CurrentCell = newRow.Cells["Name_product"];

            dataGridViewProducts.BeginEdit(true);
        }

        private void textBoxSearchProducts_TextChanged(object sender, EventArgs e)
        {
            tableProducts.SearchProducts(dataGridViewProducts, textBoxSearchProducts);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            tableHistoryCost.SearchHistoryCost(dataGridViewHistoryCost, textBoxSearchHistoryCost);
        }

        private void buttonRefreshBranchesProducts_Click(object sender, EventArgs e)
        {
            tableBranchesProducts.RefreshDataGridView(dataGridViewBranchesProducts);
        }

        private void buttonDeleteBranchesProducts_Click(object sender, EventArgs e)
        {
            tableBranchesProducts.DeleteBranchesProducts(dataGridViewBranchesProducts);
        }

        private void buttonSaveBranchesProducts_Click(object sender, EventArgs e)
        {
            tableBranchesProducts.parametersForProducts(dataGridViewBranchesProducts);
        }

        private void buttonAddBranchesProducts_Click(object sender, EventArgs e)
        {
            rowIndex = dataGridViewBranchesProducts.Rows.Add();

            DataGridViewRow newRow = dataGridViewBranchesProducts.Rows[rowIndex];

            dataGridViewBranchesProducts.CurrentCell = newRow.Cells["Id_branch"];

            dataGridViewBranchesProducts.BeginEdit(true);
        }

        private void textBoxSearchBranchesProducts_TextChanged(object sender, EventArgs e)
        {
            tableBranchesProducts.SearchBranchesProducts(dataGridViewBranchesProducts, textBoxSearchBranchesProducts);
        }

        private void buttonRefreshBranches_Click(object sender, EventArgs e)
        {
            tableBranches.RefreshDataGridView(dataGridViewBranches);
        }

        private void buttonDeleteBranches_Click(object sender, EventArgs e)
        {
            tableBranches.DeleteBranches(dataGridViewBranches);
        }

        private void buttonAddBranches_Click(object sender, EventArgs e)
        {
            rowIndex = dataGridViewBranches.Rows.Add();

            DataGridViewRow newRow = dataGridViewBranches.Rows[rowIndex];

            dataGridViewBranches.CurrentCell = newRow.Cells["Street_branches"];

            dataGridViewBranches.BeginEdit(true);
        }

        private void buttonSaveBranches_Click(object sender, EventArgs e)
        {
            tableBranches.parametersForBranches(dataGridViewBranches);
        }

        private void textBoxSearchBranches_TextChanged(object sender, EventArgs e)
        {
            tableBranches.SearchBranches(dataGridViewBranches, textBoxSearchBranches);
        }

        private void buttonRefreshOrders_Click(object sender, EventArgs e)
        {
            tableOrders.RefreshDataGridView(dataGridViewOrders);
        }

        private void buttonDeleteOrders_Click(object sender, EventArgs e)
        {
            tableOrders.DeleteOrders(dataGridViewOrders);
        }

        private void buttonAddOrders_Click(object sender, EventArgs e)
        {
            rowIndex = dataGridViewOrders.Rows.Add();

            DataGridViewRow newRow = dataGridViewOrders.Rows[rowIndex];

            dataGridViewOrders.CurrentCell = newRow.Cells["Id_suppliers"];

            dataGridViewOrders.BeginEdit(true);
        }

        private void buttonSaveOrders_Click(object sender, EventArgs e)
        {
            tableOrders.parametersForOrders(dataGridViewOrders);
        }

        private void textBoxSearchOrders_TextChanged(object sender, EventArgs e)
        {
            tableOrders.SearchOrders(dataGridViewOrders, textBoxSearchOrders);
        }

        private void buttonRefreshProductsInOrders_Click(object sender, EventArgs e)
        {
            tableProductsInOrders.RefreshDataGridView(dataGridViewProductsInOrders);
        }

        private void buttonDeleteProductsInOrders_Click(object sender, EventArgs e)
        {
            tableProductsInOrders.DeleteProductsInOrders(dataGridViewProductsInOrders);
        }

        private void buttonSaveProductsInOrders_Click(object sender, EventArgs e)
        {
            tableProductsInOrders.parametersForProductsInOrders(dataGridViewProductsInOrders);
        }

        private void buttonAddProductsInOrders_Click(object sender, EventArgs e)
        {
            rowIndex = dataGridViewProductsInOrders.Rows.Add();

            DataGridViewRow newRow = dataGridViewProductsInOrders.Rows[rowIndex];

            dataGridViewProductsInOrders.CurrentCell = newRow.Cells["Id_order"];

            dataGridViewProductsInOrders.BeginEdit(true);
        }

        private void textBoxSearchProductsInOrders_TextChanged(object sender, EventArgs e)
        {
            tableProductsInOrders.SearchProductsInOrders(dataGridViewProductsInOrders, textBoxSearchProductsInOrders);
        }

        private void buttonRefreshSuppliers_Click(object sender, EventArgs e)
        {
            tableSuppliers.RefreshDataGridView(dataGridViewSuppliers);
        }

        private void buttonSaveSuppliers_Click(object sender, EventArgs e)
        {
            tableSuppliers.parametersForSuppliers(dataGridViewSuppliers);
        }

        private void buttonAddSuppliers_Click(object sender, EventArgs e)
        {
            rowIndex = dataGridViewSuppliers.Rows.Add();

            DataGridViewRow newRow = dataGridViewSuppliers.Rows[rowIndex];

            dataGridViewSuppliers.CurrentCell = newRow.Cells["Name_suppliers"];

            dataGridViewSuppliers.BeginEdit(true);
        }

        private void buttonDeleteSuppliers_Click(object sender, EventArgs e)
        {
            tableSuppliers.DeleteSuppliers(dataGridViewSuppliers);
        }

        private void textBoxSearchSuppliers_TextChanged(object sender, EventArgs e)
        {
            tableSuppliers.SearchSuppliers(dataGridViewSuppliers, textBoxSearchSuppliers);
        }

        private void textBoxSearchSales_TextChanged(object sender, EventArgs e)
        {
            tableSales.SearchSales(dataGridViewSales, textBoxSearchSales);
        }

        private void buttonDeleteExpiredProduct_Click(object sender, EventArgs e)
        {
            tableProducts.DeleteExpiredProduct();
        } 

        private void button1_Click(object sender, EventArgs e)
        {
            tableProducts.RefreshPriceForCountry();

            tableProducts.RefreshDataGridView(dataGridViewProducts);
        }

        private void авторизацияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_authorization form = new Form_authorization();
            this.Hide();
            form.Show();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void buttonDeleteEmployees_Click_1(object sender, EventArgs e)
        {
            tableEmployees.DeleteEmployees(dataGridViewEmployees);

            tableEmployees.RefreshDataGridView(dataGridViewEmployees);
        }

        private void buttonSaveEmployees_Click_1(object sender, EventArgs e)
        {
            tableEmployees.parametersForEmployees(dataGridViewEmployees);
        }

    }
}
