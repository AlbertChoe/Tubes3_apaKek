import os
import mysql.connector
from faker import Faker
import random

fake_id = Faker('id_ID')
fake_us = Faker('en_US')


def insert_sidik_jari(cursor, nama, sidik_jari_path):
    # print(f"Inserting: {nama}, Path: {sidik_jari_path}")
    cursor.execute(
        "INSERT INTO sidik_jari (berkas_citra, nama) VALUES (%s, %s)", (sidik_jari_path, nama))


def process_files(directory):
    db_config = {
        'user': 'root',
        'password': '',  # Ganti dengan password Anda
        'host': '127.0.0.1',
        'database': 'tubes3',  # Sesuaikan dengan nama database Anda
    }
    try:
        db = mysql.connector.connect(**db_config)
        print("Database connection successful")
    except Exception as e:
        print(f"Failed to connect to database: {e}")
        return

    cursor = db.cursor()

    # Dictionary untuk menyimpan nama yang dihasilkan berdasarkan identifier
    name_dict = {}

    for root, dirs, files in os.walk(directory):
        print(f"Checking directory: {root}")
        for filename in files:
            if filename.lower().endswith(".bmp"):
                identifier = filename.split('__')[0]
                if identifier not in name_dict:
                    fake = random.choice([fake_id, fake_us])
                    name_dict[identifier] = fake.name()

                nama = name_dict[identifier]
                sidik_jari_path = os.path.join(root, filename)
                insert_sidik_jari(cursor, nama, sidik_jari_path)

    db.commit()
    cursor.close()
    db.close()


if __name__ == '__main__':
    process_files('./test/Real')
