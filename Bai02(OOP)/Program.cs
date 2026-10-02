using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace AutoSpeed
{
    public abstract class PhuongTien
    {
        private string _maPT = "PT000";
        private string _tenHang = string.Empty;
        private int _namSanXuat;
        private decimal _giaGoc;

        public string MaPT
        {
            get => _maPT;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Mã phương tiện không được để trống!");
                _maPT = value.Trim();
            }
        }

        public string TenHang
        {
            get => _tenHang;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Tên hãng không được để trống!");
                _tenHang = value.Trim();
            }
        }

        public int NamSanXuat
        {
            get => _namSanXuat;
            set
            {
                if (value < 1900 || value > DateTime.Now.Year)
                    throw new ArgumentException("Năm sản xuất không hợp lệ!");
                _namSanXuat = value;
            }
        }

        public decimal GiaGoc
        {
            get => _giaGoc;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Giá gốc phải lớn hơn 0!");
                _giaGoc = value;
            }
        }

        protected PhuongTien(string maPT, string tenHang, int namSanXuat, decimal giaGoc)
        {
            MaPT = maPT;
            TenHang = tenHang;
            NamSanXuat = namSanXuat;
            GiaGoc = giaGoc;
        }

        public abstract decimal TinhGiaLanBanh();

        public virtual string GetInfo()
        {
            return $"Mã: {MaPT} | Hãng: {TenHang} | Năm SX: {NamSanXuat} | Giá gốc: {GiaGoc:N0} VNĐ";
        }
    }

    public class OTo : PhuongTien
    {
        private int _soChoNgoi;
        private double _dungTichDongCo;

        public int SoChoNgoi
        {
            get => _soChoNgoi;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Số chỗ ngồi phải lớn hơn 0!");
                _soChoNgoi = value;
            }
        }

        public double DungTichDongCo
        {
            get => _dungTichDongCo;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Dung tích động cơ phải lớn hơn 0!");
                _dungTichDongCo = value;
            }
        }

        public OTo(string maPT, string tenHang, int namSanXuat, decimal giaGoc,
                   int soChoNgoi, double dungTichDongCo)
            : base(maPT, tenHang, namSanXuat, giaGoc)
        {
            SoChoNgoi = soChoNgoi;
            DungTichDongCo = dungTichDongCo;
        }

        public override decimal TinhGiaLanBanh()
        {
            if (SoChoNgoi <= 9)
            {
                // Giá gốc + 12% trước bạ + 30% thuế tiêu thụ đặc biệt
                return GiaGoc + GiaGoc * 0.12m + GiaGoc * 0.30m;
            }
            // Giá gốc + 10% trước bạ
            return GiaGoc + GiaGoc * 0.10m;
        }

        public override string GetInfo()
        {
            return base.GetInfo() + $" | Số chỗ: {SoChoNgoi} | Dung tích động cơ: {DungTichDongCo} L";
        }
    }

    // ===================== C. LỚP XE MÁY =====================
    public class XeMay : PhuongTien
    {
        private int _dungTichXylanh;

        public int DungTichXylanh
        {
            get => _dungTichXylanh;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Dung tích xylanh phải lớn hơn 0!");
                _dungTichXylanh = value;
            }
        }

        public XeMay(string maPT, string tenHang, int namSanXuat, decimal giaGoc, int dungTichXylanh)
            : base(maPT, tenHang, namSanXuat, giaGoc)
        {
            DungTichXylanh = dungTichXylanh;
        }

        public override decimal TinhGiaLanBanh()
        {
            if (DungTichXylanh < 175)
                return GiaGoc + GiaGoc * 0.02m;   
            return GiaGoc + GiaGoc * 0.05m;       
        }

        public override string GetInfo()
        {
            return base.GetInfo() + $" | Dung tích xylanh: {DungTichXylanh} cc";
        }
    }

    public class QuanLyPhuongTien
    {
        private readonly List<PhuongTien> _danhSach = new List<PhuongTien>();

        public void AddPhuongTien(PhuongTien pt)
        {
            if (pt == null)
                throw new ArgumentNullException(nameof(pt));
            _danhSach.Add(pt);
        }

        public void DisplayAll()
        {
            if (_danhSach.Count == 0)
            {
                Console.WriteLine("Danh sách phương tiện trống.");
                return;
            }

            foreach (PhuongTien pt in _danhSach)
            {
                Console.WriteLine(pt.GetInfo());
                Console.WriteLine($"   => Giá lăn bánh: {pt.TinhGiaLanBanh():N0} VNĐ");
            }
        }

        public PhuongTien FindMaxGiaLanBanh()
        {
            if (_danhSach.Count == 0) return null;

            PhuongTien max = _danhSach[0];
            foreach (PhuongTien pt in _danhSach)
            {
                if (pt.TinhGiaLanBanh() > max.TinhGiaLanBanh())
                    max = pt;
            }
            return max;
        }

        public List<PhuongTien> SearchByName(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return new List<PhuongTien>();

            return _danhSach
                .Where(pt => pt.TenHang.IndexOf(keyword.Trim(), StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
        }
    }

    // ===================== CHƯƠNG TRÌNH CHÍNH (MENU) =====================
    public class Program
    {
        private static readonly QuanLyPhuongTien ql = new QuanLyPhuongTien();

        public static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            bool chayTiep = true;
            while (chayTiep)
            {
                HienMenu();
                string chon = Console.ReadLine();
                Console.WriteLine();

                switch (chon != null ? chon.Trim() : "")
                {
                    case "1": ThemOTo(); break;
                    case "2": ThemXeMay(); break;
                    case "3": ql.DisplayAll(); break;
                    case "4": TimGiaMax(); break;
                    case "5": TimTheoTen(); break;
                    case "6": ChayTestMau(); break;
                    case "0": chayTiep = false; continue;
                    default: Console.WriteLine("Lựa chọn không hợp lệ, vui lòng chọn lại!"); break;
                }

                Console.WriteLine("\nNhấn Enter để quay lại menu...");
                Console.ReadLine();
            }
        }

        private static void HienMenu()
        {
            Console.Clear();
            Console.WriteLine("========== QUẢN LÝ PHƯƠNG TIỆN AUTOSPEED ==========");
            Console.WriteLine("1. Thêm Ô tô");
            Console.WriteLine("2. Thêm Xe máy");
            Console.WriteLine("3. Hiển thị toàn bộ phương tiện (kèm giá lăn bánh)");
            Console.WriteLine("4. Tìm phương tiện có giá lăn bánh cao nhất");
            Console.WriteLine("5. Tìm phương tiện theo tên hãng");
            Console.WriteLine("6. Chạy test case mẫu (TC01 - TC05)");
            Console.WriteLine("0. Thoát");
            Console.Write("Chọn chức năng: ");
        }
        private static void ThemOTo()
        {
            Console.WriteLine("--- THÊM Ô TÔ ---");
            string ma = NhapChuoi("Mã PT: ");
            string hang = NhapChuoi("Tên hãng: ");
            int nam = NhapSoNguyen("Năm sản xuất: ");
            decimal gia = NhapTien("Giá gốc (VNĐ, chỉ nhập số): ");
            int soCho = NhapSoNguyen("Số chỗ ngồi: ");
            double dungTich = NhapSoThuc("Dung tích động cơ (lít): ");

            try
            {
                ql.AddPhuongTien(new OTo(ma, hang, nam, gia, soCho, dungTich));
                Console.WriteLine("=> Thêm Ô tô thành công!");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("=> Không thêm được: " + ex.Message);
            }
        }

        private static void ThemXeMay()
        {
            Console.WriteLine("--- THÊM XE MÁY ---");
            string ma = NhapChuoi("Mã PT: ");
            string hang = NhapChuoi("Tên hãng: ");
            int nam = NhapSoNguyen("Năm sản xuất: ");
            decimal gia = NhapTien("Giá gốc (VNĐ, chỉ nhập số): ");
            int cc = NhapSoNguyen("Dung tích xylanh (cc): ");

            try
            {
                ql.AddPhuongTien(new XeMay(ma, hang, nam, gia, cc));
                Console.WriteLine("=> Thêm Xe máy thành công!");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("=> Không thêm được: " + ex.Message);
            }
        }

        private static void TimGiaMax()
        {
            PhuongTien max = ql.FindMaxGiaLanBanh();
            if (max == null)
            {
                Console.WriteLine("Danh sách trống, hãy thêm phương tiện trước.");
                return;
            }
            Console.WriteLine("Phương tiện có giá lăn bánh cao nhất:");
            Console.WriteLine(max.GetInfo());
            Console.WriteLine("   => Giá lăn bánh: " + max.TinhGiaLanBanh().ToString("N0") + " VNĐ");
        }

        private static void TimTheoTen()
        {
            string tuKhoa = NhapChuoi("Nhập tên hãng cần tìm: ");
            List<PhuongTien> kq = ql.SearchByName(tuKhoa);
            if (kq.Count == 0)
            {
                Console.WriteLine("Không tìm thấy phương tiện nào.");
                return;
            }
            Console.WriteLine("Tìm thấy " + kq.Count + " phương tiện:");
            foreach (PhuongTien pt in kq)
            {
                Console.WriteLine(pt.GetInfo());
                Console.WriteLine("   => Giá lăn bánh: " + pt.TinhGiaLanBanh().ToString("N0") + " VNĐ");
            }
        }

        // ---------- Test case mẫu TC01 - TC05 ----------
        private static void ChayTestMau()
        {
            Console.WriteLine("=== TC01: Validation Năm sản xuất ===");
            try
            {
                new OTo("OT00", "Toyota", 1850, 1000000000m, 5, 2.0);
                Console.WriteLine("Tạo được đối tượng (SAI so với kỳ vọng!)");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("ArgumentException: " + ex.Message);
            }

            Console.WriteLine("\n=== TC02: Giá lăn bánh Ô tô 5 chỗ ===");
            OTo oto = new OTo("OT01", "Toyota Camry", 2023, 1000000000m, 5, 2.5);
            Console.WriteLine("Giá lăn bánh: " + oto.TinhGiaLanBanh().ToString("N0") + " VNĐ (kỳ vọng 1,420,000,000)");

            Console.WriteLine("\n=== TC03: Giá lăn bánh Xe máy 150cc ===");
            XeMay xeMay = new XeMay("XM01", "Honda Winner", 2024, 50000000m, 150);
            Console.WriteLine("Giá lăn bánh: " + xeMay.TinhGiaLanBanh().ToString("N0") + " VNĐ (kỳ vọng 51,000,000)");

            Console.WriteLine("\n=== TC04: Đa hình List<PhuongTien> ===");
            List<PhuongTien> ds = new List<PhuongTien> { oto, xeMay };
            foreach (PhuongTien pt in ds)
                Console.WriteLine(pt.GetType().Name + " - " + pt.TenHang + ": " + pt.TinhGiaLanBanh().ToString("N0") + " VNĐ");

            Console.WriteLine("\n=== TC05: Tìm giá lăn bánh cao nhất ===");
            QuanLyPhuongTien qlTest = new QuanLyPhuongTien();
            qlTest.AddPhuongTien(oto);
            qlTest.AddPhuongTien(xeMay);
            PhuongTien max = qlTest.FindMaxGiaLanBanh();
            Console.WriteLine(max.GetInfo());
            Console.WriteLine("Giá lăn bánh: " + max.TinhGiaLanBanh().ToString("N0") + " VNĐ");
        }

        // ---------- Hàm hỗ trợ nhập liệu (nhập sai thì bắt nhập lại) ----------
        private static string NhapChuoi(string thongBao)
        {
            Console.Write(thongBao);
            string s = Console.ReadLine();
            return s == null ? "" : s.Trim();
        }

        private static int NhapSoNguyen(string thongBao)
        {
            while (true)
            {
                Console.Write(thongBao);
                int kq;
                if (int.TryParse(Console.ReadLine(), out kq)) return kq;
                Console.WriteLine("Giá trị không hợp lệ, vui lòng nhập một số nguyên!");
            }
        }

        private static double NhapSoThuc(string thongBao)
        {
            while (true)
            {
                Console.Write(thongBao);
                string s = (Console.ReadLine() ?? "").Trim().Replace(',', '.');
                double kq;
                if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out kq)) return kq;
                Console.WriteLine("Giá trị không hợp lệ, vui lòng nhập một số (ví dụ 2.5)!");
            }
        }

        private static decimal NhapTien(string thongBao)
        {
            while (true)
            {
                Console.Write(thongBao);
                string s = (Console.ReadLine() ?? "").Replace(".", "").Replace(",", "").Replace(" ", "");
                decimal kq;
                if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out kq)) return kq;
                Console.WriteLine("Giá trị không hợp lệ, vui lòng nhập số tiền (ví dụ 1000000000)!");
            }
        }
    }
}