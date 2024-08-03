import sqlite3
import datetime

con = sqlite3.connect("..\\myexpenses-backup-20240123-143443\\BACKUP")
cur = con.cursor()

# tables list
# print(cur.execute("SELECT name FROM sqlite_schema WHERE type ='table' AND name NOT LIKE 'sqlite_%';").fetchall())

# for row in cur.execute("SELECT * FROM sqlite_master where type = 'table'"):
#     print(row)

# ('table', 'categories', 'categories', 20, 'CREATE TABLE categories (_id integer primary key autoincrement, label text not null, label_normalized text,
#  parent_id integer references categories(_id) ON DELETE CASCADE, usages integer default 0, last_used datetime, color integer, icon string, uuid text, type integer, 
#  UNIQUE (label,parent_id))')
categories = { }
for row in cur.execute("SELECT * FROM categories"):
    id = row[0]
    label_normalized = row[2]
    # print(f"{id}\t{label_normalized}")
    categories[id] = label_normalized

# ('table', 'transactions', 'transactions', 4, "CREATE TABLE transactions( _id integer primary key autoincrement, comment text, date datetime not null, 
# value_date datetime not null, amount integer not null, cat_id integer references categories(_id), account_id integer not null references accounts(_id) 
# ON DELETE CASCADE,payee_id integer references payee(_id), transfer_peer integer references transactions(_id), transfer_account integer references accounts(_id),
# method_id integer references paymentmethods(_id),parent_id integer references transactions(_id) ON DELETE CASCADE, status integer default 0, 
# cr_status text not null check (cr_status in ('UNRECONCILED','CLEARED','RECONCILED','VOID')) default 'RECONCILED',number text, uuid text, original_amount integer,
# original_currency text, equivalent_amount integer,  debt_id integer references debts(_id) ON DELETE SET NULL)")     
categories_sum = { }
for row in cur.execute("SELECT * FROM transactions"):
    date = datetime.datetime.fromtimestamp(row[2])
    id = row[0]
    amount = row[4]
    cat_id = row[5]
    # cat_label = categories[cat_id]
    # print(f"{id}\t{date}\t{amount}\t{cat_label}")
    if cat_id in categories_sum:
        categories_sum[cat_id] = categories_sum[cat_id] + amount
    else:
        categories_sum[cat_id] = amount

for id in categories_sum:
    print(f"{categories[id]}\t{categories_sum[id]}")