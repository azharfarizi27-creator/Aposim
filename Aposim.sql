CREATE DATABASE posinventory;
USE posinventory;

-- 1. Tabel Role & User
CREATE TABLE roles (
    role_id INT AUTO_INCREMENT PRIMARY KEY,
    role_name VARCHAR(50) NOT NULL,
    description VARCHAR(255)
);

CREATE TABLE users (
    user_id INT AUTO_INCREMENT PRIMARY KEY,
    nama VARCHAR(100) NOT NULL,
    username VARCHAR(50) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    email VARCHAR(100),
    no_hp VARCHAR(20),
    role_id INT,
    STATUS ENUM('active', 'inactive') DEFAULT 'active',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (role_id) REFERENCES roles(role_id)
);

-- 2. Kategori dan Produk
CREATE TABLE categories (
    category_id INT AUTO_INCREMENT PRIMARY KEY,
    category_name VARCHAR(100) NOT NULL,
    description TEXT
);

CREATE TABLE products (
    product_id INT AUTO_INCREMENT PRIMARY KEY,
    product_code VARCHAR(50) UNIQUE NOT NULL,
    product_name VARCHAR(150) NOT NULL,
    category_id INT,
    purchase_price DECIMAL(15,2) NOT NULL,
    selling_price DECIMAL(15,2) NOT NULL,
    stock INT DEFAULT 0,
    min_stock INT DEFAULT 5,
    unit VARCHAR(50),
    STATUS ENUM('active','inactive') DEFAULT 'active',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (category_id) REFERENCES categories(category_id)
);

-- 3. Supplier & Customer
CREATE TABLE suppliers (
    supplier_id INT AUTO_INCREMENT PRIMARY KEY,
    supplier_name VARCHAR(100) NOT NULL,
    contact_name VARCHAR(100),
    phone VARCHAR(20),
    email VARCHAR(100),
    address TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE customers (
    customer_id INT AUTO_INCREMENT PRIMARY KEY,
    customer_name VARCHAR(100) NOT NULL,
    phone VARCHAR(20),
    email VARCHAR(100),
    address TEXT,
    member_points INT DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 4. Transaksi Pembelian
CREATE TABLE purchases (
    purchase_id INT AUTO_INCREMENT PRIMARY KEY,
    supplier_id INT,
    invoice_number VARCHAR(50) UNIQUE,
    purchase_date DATETIME DEFAULT CURRENT_TIMESTAMP,
    total_amount DECIMAL(15,2),
    payment_status ENUM('lunas','belum lunas') DEFAULT 'belum lunas',
    created_by INT,
    FOREIGN KEY (supplier_id) REFERENCES suppliers(supplier_id),
    FOREIGN KEY (created_by) REFERENCES users(user_id)
);

CREATE TABLE purchase_items (
    purchase_item_id INT AUTO_INCREMENT PRIMARY KEY,
    purchase_id INT,
    product_id INT,
    quantity INT NOT NULL,
    price DECIMAL(15,2) NOT NULL,
    subtotal DECIMAL(15,2) NOT NULL,
    FOREIGN KEY (purchase_id) REFERENCES purchases(purchase_id),
    FOREIGN KEY (product_id) REFERENCES products(product_id)
);

-- 5. Transaksi Penjualan
CREATE TABLE sales (
    sale_id INT AUTO_INCREMENT PRIMARY KEY,
    customer_id INT,
    invoice_number VARCHAR(50) UNIQUE,
    sale_date DATETIME DEFAULT CURRENT_TIMESTAMP,
    total_amount DECIMAL(15,2),
    payment_method ENUM('cash','debit','qris','ewallet','credit') DEFAULT 'cash',
    paid_amount DECIMAL(15,2),
    change_amount DECIMAL(15,2),
    created_by INT,
    FOREIGN KEY (customer_id) REFERENCES customers(customer_id),
    FOREIGN KEY (created_by) REFERENCES users(user_id)
);

CREATE TABLE sale_items (
    sale_item_id INT AUTO_INCREMENT PRIMARY KEY,
    sale_id INT,
    product_id INT,
    quantity INT NOT NULL,
    price DECIMAL(15,2) NOT NULL,
    discount DECIMAL(15,2) DEFAULT 0,
    subtotal DECIMAL(15,2) NOT NULL,
    FOREIGN KEY (sale_id) REFERENCES sales(sale_id),
    FOREIGN KEY (product_id) REFERENCES products(product_id)
);

-- 6. Stok & Penyesuaian
CREATE TABLE stock_adjustments (
    adjustment_id INT AUTO_INCREMENT PRIMARY KEY,
    product_id INT,
    adjustment_type ENUM('in','out') NOT NULL,
    quantity INT NOT NULL,
    note TEXT,
    created_by INT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (product_id) REFERENCES products(product_id),
    FOREIGN KEY (created_by) REFERENCES users(user_id)
);

-- 7. Kas & Keuangan
CREATE TABLE cashflows (
    cashflow_id INT AUTO_INCREMENT PRIMARY KEY,
    TYPE ENUM('in','out') NOT NULL,
    description VARCHAR(255),
    amount DECIMAL(15,2) NOT NULL,
    created_by INT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (created_by) REFERENCES users(user_id)
);

-- 8. Log Aktivitas
CREATE TABLE LOGS (
    log_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT,
    ACTION VARCHAR(255),
    log_time TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);

-- 9. Pengaturan Sistem
CREATE TABLE settings (
    setting_id INT AUTO_INCREMENT PRIMARY KEY,
    store_name VARCHAR(150),
    store_address TEXT,
    store_phone VARCHAR(20),
    store_logo VARCHAR(255),
    npwp VARCHAR(50)
);

INSERT INTO roles (role_name, description) VALUES
('Admin', 'Akses penuh ke seluruh sistem'),
('Kasir', 'Akses transaksi penjualan'),
('Manajer', 'Akses laporan dan monitoring');

INSERT INTO users (nama, username, password_hash, email, no_hp, role_id, STATUS) VALUES
(
  'Administrator',
  'admin',
  SHA2(CONCAT('admin','123'), 256),
  'admin@toko.com',
  '0811111111',
  1,
  'active'
),
(
  'Kasir Utama',
  'kasir',
  SHA2(CONCAT('kasir','123'), 256),
  'kasir@toko.com',
  '0822222222',
  2,
  'active'
),
(
  'Manajer Toko',
  'manajer',
  SHA2(CONCAT('manajer','123'), 256),
  'manajer@toko.com',
  '0833333333',
  3,
  'active'
);


INSERT INTO categories (category_name, description) VALUES
('Minuman', 'Kategori minuman kemasan'),
('Makanan', 'Kategori makanan ringan'),
('ATK', 'Alat tulis kantor'),
('Household', 'Kebutuhan rumah tangga');

INSERT INTO customers (customer_name, phone, email, address, member_points) VALUES
('Customer 1', '0800000000', 'customer@toko.com', 'Indonesia', 0);

INSERT INTO suppliers (supplier_name, contact_name, phone, email, address) VALUES
('Supplier 1', 'Budi Supplier', '0899999999', 'supplier@toko.com', 'Jakarta');

INSERT INTO products 
(product_code, product_name, category_id, purchase_price, selling_price, stock, min_stock, unit, STATUS)
VALUES
('PRD001', 'Air Mineral 600ml', 1, 3000, 5000, 100, 10, 'Botol', 'active'),
('PRD002', 'Snack Ring', 2, 5000, 8000, 50, 10, 'Pack', 'active'),
('PRD003', 'Pulpen Biru', 3, 2000, 4000, 30, 5, 'Pcs', 'active');

INSERT INTO cashflows (TYPE, description, amount, created_by) VALUES
('in', 'Kas awal toko', 500000000, 1);

INSERT INTO settings 
(store_name, store_address, store_phone, store_logo, npwp)
VALUES
('Toko POS Inventory',
 'Jl. Contoh No. 123',
 'null',
 '021123456',
 '12.345.678.9-012.345'
);
 
 INSERT INTO LOGS (user_id, ACTION) VALUES
(1, 'Inisialisasi database dan data awal');

INSERT INTO products
(product_code, product_name, category_id, purchase_price, selling_price, stock, min_stock, unit, STATUS)
VALUES
-- MAKANAN
('PRD008', 'Mie Instan Kuah', 2, 2400, 3500, 120, 20, 'Pcs', 'active'),
('PRD009', 'Biskuit Coklat', 2, 6000, 9000, 40, 10, 'Pack', 'active'),
('PRD010', 'Wafer Keju', 2, 5000, 8000, 50, 10, 'Pack', 'active'),
('PRD011', 'Roti Tawar', 2, 11000, 15000, 25, 5, 'Pack', 'active'),
('PRD012', 'Snack Kentang', 2, 7000, 10000, 35, 10, 'Pack', 'active'),

-- MINUMAN
('PRD013', 'Kopi Hitam Sachet', 1, 1200, 2000, 200, 30, 'Sachet', 'active'),
('PRD014', 'Kopi Susu Sachet', 1, 1500, 2500, 180, 30, 'Sachet', 'active'),
('PRD015', 'Air Mineral 1,5L', 1, 5000, 8000, 60, 10, 'Botol', 'active'),
('PRD016', 'Minuman Energi', 1, 7000, 11000, 40, 10, 'Botol', 'active'),
('PRD017', 'Susu UHT 250ml', 1, 6000, 9000, 50, 10, 'Kotak', 'active'),

-- HOUSEHOLD / ATK
('PRD018', 'Sabun Cuci Piring', 3, 7000, 11000, 30, 5, 'Pcs', 'active'),
('PRD019', 'Sabun Mandi Cair', 3, 9000, 14000, 25, 5, 'Pcs', 'active'),
('PRD020', 'Pasta Gigi', 3, 8000, 13000, 20, 5, 'Pcs', 'active'),
('PRD021', 'Sikat Gigi', 3, 5000, 9000, 30, 5, 'Pcs', 'active'),
('PRD022', 'Tisu Gulung', 3, 10000, 16000, 20, 5, 'Roll', 'active');


