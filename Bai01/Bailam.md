# Bài làm - Bài 01

## Câu 1: Value Types và Reference Types (Stack vs Heap)

| Tiêu chí | Value Type | Reference Type |
|---|---|---|
| Ví dụ | `int`, `double`, `bool`, `char`, `struct`, `enum` | `class`, `string`, `array`, `interface`, `delegate`, `object` |
| Nội dung biến chứa | Chính giá trị dữ liệu | Địa chỉ (tham chiếu) trỏ tới đối tượng |
| Vùng nhớ | Thường nằm trên **Stack** (khi là biến cục bộ) | Đối tượng nằm trên **Heap**, biến tham chiếu nằm trên Stack |
| Khi gán `b = a` | Sao chép giá trị, `a` và `b` độc lập | Sao chép địa chỉ, `a` và `b` cùng trỏ một đối tượng |
| Giá trị `null` | Không nhận `null` (trừ khi dùng `int?`) | Có thể nhận `null` |
| Giải phóng bộ nhớ | Tự động khi ra khỏi phạm vi (scope) | Do Garbage Collector (GC) dọn dẹp |

## Câu 2: Init-only Properties (`init`)

**Khác biệt:**
- Thuộc tính `set` thông thường: có thể gán lại giá trị **bất cứ lúc nào**.
- Thuộc tính `init` (C# 9): chỉ được gán trong **object initializer**, **constructor** hoặc `with` expression. Sau khi đối tượng được tạo xong thì **không sửa được nữa** (bất biến - immutable).

**Trường hợp sử dụng thực tế:**
- Các đối tượng dữ liệu không nên thay đổi sau khi tạo: DTO, model trả về từ API, cấu hình ứng dụng (config).
- Thuộc tính định danh như `Id`, `MaSV`, `Email` chỉ gán một lần.
- Tạo đối tượng bất biến mà vẫn dùng được cú pháp object initializer gọn gàng (không cần viết constructor nhiều tham số). Rất hay dùng với `record`.
- Giúp an toàn khi chạy đa luồng vì dữ liệu không bị sửa ngoài ý muốn.

---

## Câu 3: `virtual` (lớp cha) và `override` (lớp con) trong Đa hình

- **`virtual`** (ở lớp cha): đánh dấu phương thức **có thể** được lớp con viết lại. Lớp cha cung cấp sẵn cài đặt mặc định.
- **`override`** (ở lớp con): **viết lại** phương thức `virtual` đó với cách xử lý riêng của lớp con.

**Cơ chế đa hình:** khi gọi phương thức qua biến kiểu lớp cha, C# quyết định chạy phiên bản nào dựa trên **kiểu thực tế của đối tượng lúc chạy** (runtime / dynamic binding), không dựa vào kiểu khai báo của biến.



| | `virtual` | `override` |
|---|---|---|
| Đặt ở | Lớp cha | Lớp con |
| Vai trò | Cho phép ghi đè, có cài đặt mặc định | Thực sự ghi đè, thay đổi hành vi |
| Bắt buộc? | Phải có thì lớp con mới `override` được | Không bắt buộc, lớp con không override thì dùng bản của cha |

**Lưu ý:** nếu lớp con dùng `new` thay cho `override` thì chỉ là **che giấu** (hiding), không có đa hình thật sự.

---

## Câu 4: Vì sao thành phần `static` không truy xuất được qua instance?

- Thành phần `static` **thuộc về chính lớp (type)**, không thuộc về từng đối tượng. Cả chương trình chỉ có **một bản duy nhất**, được dùng chung cho mọi đối tượng.
- Khi dùng `new`, mỗi đối tượng chỉ chứa các thành phần **instance** (không static) của riêng nó. Thành phần static không nằm trong đối tượng đó.
- Truy xuất qua instance sẽ gây nhầm lẫn, vì tưởng đó là dữ liệu riêng của đối tượng trong khi thực tế là dữ liệu chung. Vì vậy C# **bắt buộc truy cập qua tên lớp** và báo lỗi biên dịch **CS0176** nếu dùng instance.
- Phương thức static cũng không có con trỏ `this`, nên không truy cập được thành phần instance trực tiếp.

