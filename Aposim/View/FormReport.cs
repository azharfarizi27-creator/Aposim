using Aposim.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Windows.Forms;

namespace Aposim.View
{
    public partial class FormReport : Form
    {
        ReportController report = new ReportController();
        enum ReportType
        {
            Produk,
            Penjualan,
            Pembelian,
            Keuangan
        }

        ReportType currentReport;
        SettingController setting = new SettingController();
        string storeName = "";
        string reportTitle = "";


        public FormReport()
        {
            InitializeComponent();

        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnProduk_Click(object sender, EventArgs e)
        {
            dgvReport.DataSource = report.GetProductReport();
            dgvReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            currentReport = ReportType.Produk;
            reportTitle = "DAFTAR BARANG TOKO";
        }

        private void btnJualbeli_Click(object sender, EventArgs e)
        {
            dgvReport.DataSource = report.GetTransactionReport();
            dgvReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            currentReport = ReportType.Penjualan;
            reportTitle = "LAPORAN PENJUALAN";
        }

        private void btnKas_Click(object sender, EventArgs e)
        {
            dgvReport.DataSource = report.GetCashflowReport();
            dgvReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            currentReport = ReportType.Keuangan;
            reportTitle = "LAPORAN KEUANGAN";
        }

        private void FormReport_Load(object sender, EventArgs e)
        {
            storeName = setting.GetStoreName();

        }

        void ExportPdfAuto()
        {
            if (dgvReport.Rows.Count == 0)
            {
                MessageBox.Show("Data kosong");
                return;
            }

            // 📁 Folder otomatis
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "AposimReport"
            );

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string fileName =
                $"{reportTitle} - {storeName} - {DateTime.Now:yyyy-MM-dd}.pdf";

            string fullPath = Path.Combine(folder, fileName);

            // 📄 PDF Document
            iTextSharp.text.Document doc =
                new iTextSharp.text.Document(PageSize.A4.Rotate(), 20, 20, 20, 20);

            PdfWriter.GetInstance(doc, new FileStream(fullPath, FileMode.Create));
            doc.Open();

            // ===== FONT =====
            var fontToko = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
            var fontJudul = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            var fontIsi = FontFactory.GetFont(FontFactory.HELVETICA, 9);

            // ===== HEADER =====
            doc.Add(new Paragraph(storeName, fontToko));
            doc.Add(new Paragraph(reportTitle, fontJudul));
            doc.Add(new Paragraph(
                "Tanggal cetak: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                fontIsi
            ));
            doc.Add(new Paragraph(" "));

            // ===== TABLE =====
            PdfPTable table = new PdfPTable(dgvReport.Columns.Count);
            table.WidthPercentage = 100;

            // Header tabel
            foreach (DataGridViewColumn col in dgvReport.Columns)
            {
                PdfPCell cell = new PdfPCell(new Phrase(col.HeaderText, fontIsi));
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
            }

            // Isi tabel
            foreach (DataGridViewRow row in dgvReport.Rows)
            {
                if (row.IsNewRow) continue;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    table.AddCell(new Phrase(cell.Value?.ToString(), fontIsi));
                }
            }

            doc.Add(table);
            doc.Close();

            MessageBox.Show(
                "PDF berhasil dibuat:\n" + fullPath,
                "Sukses",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }



        void ExportExcelAuto()
        {
            if (dgvReport.Rows.Count == 0)
            {
                MessageBox.Show("Data kosong");
                return;
            }


            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "AposimReport"
            );

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string fileName =
                $"{reportTitle} - {storeName} - {DateTime.Now:yyyy-MM-dd}.xlsx";

            string fullPath = Path.Combine(folder, fileName);

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add("Report");

                int row = 1;
                int col = 1;

                // ===== JUDUL =====
                ws.Cells[row, col].Value = storeName;
                ws.Cells[row, col].Style.Font.Bold = true;
                ws.Cells[row, col].Style.Font.Size = 14;

                row++;

                ws.Cells[row, col].Value = reportTitle;
                ws.Cells[row, col].Style.Font.Bold = true;

                row += 2;

                // ===== HEADER =====
                foreach (DataGridViewColumn dgvCol in dgvReport.Columns)
                {
                    ws.Cells[row, col].Value = dgvCol.HeaderText;
                    ws.Cells[row, col].Style.Font.Bold = true;
                    ws.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    col++;
                }

                row++;
                col = 1;

                // ===== ISI =====
                foreach (DataGridViewRow dgvRow in dgvReport.Rows)
                {
                    if (dgvRow.IsNewRow) continue;

                    foreach (DataGridViewCell cell in dgvRow.Cells)
                    {
                        ws.Cells[row, col].Value = cell.Value;
                        ws.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        col++;
                    }

                    col = 1;
                    row++;
                }

                ws.Cells.AutoFitColumns();

                FileInfo file = new FileInfo(fullPath);
                package.SaveAs(file);
            }

            MessageBox.Show(
                "Excel berhasil dibuat:\n" + fullPath,
                "Sukses",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }



        private void btnExport_Click(object sender, EventArgs e)
        {
            ExportPdfAuto();
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            ExportExcelAuto();
        }

        private void lblRole_Click(object sender, EventArgs e)
        {

        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
