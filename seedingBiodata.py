import mysql.connector
from faker import Faker
import random

fake = Faker('id_ID')


def corrupt_name(name):

    def mix_case(name):
        return ''.join(random.choice([str.upper, str.lower])(char) for char in name)

    def replace_with_numbers(name):
        return name.replace('a', '4').replace('i', '1').replace('e', '3').replace('o', '0').replace('s', '5')

    def abbreviate(name):
        return ''.join(char for char in name if char.lower() not in 'aeiou')

    transformations = [mix_case, replace_with_numbers, abbreviate]

    num_transforms = random.randint(1, len(transformations))
    selected_transforms = random.sample(
        transformations, num_transforms)

    result = name
    for transform in selected_transforms:
        result = transform(result)

    return result


def connect_to_db():
    return mysql.connector.connect(
        host="localhost",
        user="root",
        password="1234",  # Enter your password
        database="tubes3"  # Your database name
    )


def process_names():
    db = connect_to_db()
    cursor = db.cursor()

    cursor.execute("SELECT DISTINCT nama FROM sidik_jari")
    names = cursor.fetchall()

    insert_query = """
    INSERT INTO biodata (NIK, nama, tempat_lahir, tanggal_lahir, jenis_kelamin, golongan_darah,
                         alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan)
    VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)
    """
    for (original_name,) in names:
        NIK = fake.unique.random_number(digits=16, fix_len=True)
        corrupted_name = corrupt_name(original_name)
        tempat_lahir = fake.city()
        tanggal_lahir = fake.date_of_birth(minimum_age=18, maximum_age=65)
        jenis_kelamin = random.choice(['Laki-Laki', 'Perempuan'])
        golongan_darah = random.choice(
            ['A', 'B', 'AB', 'O']) + random.choice(['+', '-'])
        alamat = fake.address()
        agama = random.choice(
            ['Islam', 'Kristen', 'Hindu', 'Buddha', 'Konghucu'])
        status_perkawinan = random.choice(
            ['Belum Menikah', 'Menikah', 'Cerai'])
        pekerjaan = fake.job()
        kewarganegaraan = 'Indonesia'

        cursor.execute(insert_query, (NIK, corrupted_name, tempat_lahir, tanggal_lahir, jenis_kelamin,
                                      golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan))

    db.commit()
    cursor.close()
    db.close()
    print("Names and additional details processed and inserted into biodata table.")


if __name__ == "__main__":
    process_names()
