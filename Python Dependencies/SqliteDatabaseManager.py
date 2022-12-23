import numpy as np
import pandas as pd 
import sqlite3 as db

class SqliteDatabaseManager:
    #dbms_path is including the db_name also
    db_path = None
    db_connection = None
    db_cursor = None

    def __init__(self, db_path) -> None:
        self.db_path = db_path
        
        self.db_connection = db.connect(self.db_path)
        self.db_cursor = self.db_connection.cursor()

    def close_db(self) -> None:
        self.db_connection.commit()
        self.db_cursor.close()
        self.db_connection.close()

    def get_data(self, table_name, df_format = False, index_col = ''):
        if not df_format:
            return pd.read_sql_query(f'SELECT * FROM {table_name}', self.db_connection).to_dict('records')
        else:
            df = pd.read_sql_query(f'SELECT * FROM {table_name}', self.db_connection)
            if index_col != '':
                df = df.set_index(index_col)
            return df

    def execute_col_query(self, query, col_name) -> list:
        return pd.read_sql_query(query, self.db_connection)[col_name].to_list()

    def execute_query(self, query, df_format = False) -> dict:
        if df_format:
            df = pd.read_sql(query, self.db_connection)
            return df
        else:
            df = pd.read_sql(query, self.db_connection).to_dict('records')
            return df

    def insert_row(self, table_name, index_col, data_dict) -> int:
        try:
            if 'OpenDate' in data_dict.keys():
                data_dict['OpenDate'] = data_dict['OpenDate'][:10]
            if 'CloseDate' in data_dict.keys():
                data_dict['CloseDate'] = data_dict['CloseDate'][:10]
            if 'Date' in data_dict.keys():
                data_dict['Date'] = data_dict['Date'][:10]
            
            query = f'INSERT OR REPLACE INTO {table_name} VALUES('
            for i in range(len(data_dict)):
                query += '?,'
            query = query[:len(query)-1]
            query += ')'
            self.db_cursor.execute(query, tuple(data_dict.values()))
            self.db_connection.commit()
        except Exception as ex:
            print(ex)
            return -1
        return 1

    def update_value(self, table_name, index_col, update_col, index_id, value) -> int:
        try:
            query = f'UPDATE {table_name} SET {update_col} = {value} WHERE {index_col} = \'{index_id}\''
            if type(value) == str:
                query = f'UPDATE {table_name} SET {update_col} = \'{value}\' WHERE {index_col} = \'{index_id}\''
            self.db_cursor.execute(query)
            self.db_connection.commit()
        except Exception as ex:
            print(ex)
            return -1
        return 1

    def delete_row(self, table_name, index_col, data_dict) -> int:
        try:
            temp_id = data_dict[index_col]
            self.db_cursor.execute(f'DELETE FROM {table_name} WHERE {index_col} = \'{temp_id}\'')
            self.db_connection.commit()
        except Exception as ex:
            print(ex)
            return -1
        return 1

    def process_df(self, df, replace_close):
        if df.index.name == 'Date':
            df.index.name = 'datetime'
        elif df.index.name == None:
            if 'Date' in df.columns:
                df.set_index('Date', inplace = True)
                df.index.name = 'datetime'
            elif 'datetime' in df.columns:
                df.set_index('datetime', inplace = True)
        df.columns = [['open', 'high', 'low', 'close', 'adj close', 'volume']]

        if type(df.index[0]) == str:
            df.index = pd.to_datetime(df.index)
        
        if replace_close and 'adj close' in df.columns:
            df['close'] = df['adj close'].values
            df = df[['open', 'high', 'low', 'close', 'volume']]
        return df







