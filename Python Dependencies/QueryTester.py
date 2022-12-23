import numpy as np
import pandas as pd 
import requests
from SqliteDatabaseManager import SqliteDatabaseManager as dbms
from datetime import date, timedelta
from DataTools import DataAnalytics as DA

path_str = 'C:\\Users\\shara\\AppData\\Local\Packages\\b135f1fa-51ac-400e-bb0c-808ff7cf55d0_gdn5w8x9s5wpa\\LocalState\\ApplicationDB.db'
db = dbms(path_str)

# row_dict = {'AssetID':'HG4J496F', 'Category':'Stocks/ETF', 'Name':'RELIANCE.NS', 'Shares':15, 'OpenDate':'2019-01-01', 'OpenPrice':'1500.64', 'CloseDate':'2022-11-25', 'ClosePrice':'2500.67', 'IsClosed':False}
# db.insert_data('FinancialAssets', 'AssetID', row_dict)

# url = 'http://127.0.0.1:8080/colquery'
# data = {'query':'SELECT DISTINCT(Category) from FinancialAssets','colname':'Category'}

# x = requests.post(url, json = data)

# print(x.text)

# fa_df = db.get_data('FinancialAssets', df_format=True)


# min_date = fa_df['OpenDate'].min()
# max_date = fa_df['CloseDate'].max()
# value_df = pd.DataFrame(columns = ['Date', 'Total Value'])
# value_df['Date'] = pd.date_range(min_date, max_date, freq='d')

# print(type(min_date))
# fa_df = fa_df.set_index('AssetID')
# fa_df.to_csv('FA_DATA.csv')

#query = ('SELECT DISTINCT(UPPER(Purpose)) AS \'Category\', (SUM(Amount) * 100 / (SELECT SUM(Amount) FROM Spending WHERE Credit != \'Credit\')) AS \'Percentage\' FROM Spending WHERE Credit != \'Credit\' GROUP BY Purpose')
#query = ('SELECT DISTINCT(UPPER(City)) AS \'Category\', (SUM(ClosePrice) * 100 / (SELECT SUM(ClosePrice) FROM RealEstates)) AS \'Percentage\' FROM RealEstates GROUP BY City')
#query = ('SELECT DISTINCT(UPPER(LoanName)) AS \'Category\', (SUM(AmountRemaining) * 100 / (SELECT SUM(AmountRemaining) FROM Liabilities)) AS \'Percentage\' FROM Liabilities GROUP BY LoanName')
#GROUP BY DISTINCT(UPPER(Purpose))

#query = 'SELECT f.Category, SUM(f.Shares * f.ClosePrice) AS \'Total Value\' FROM FinancialAssets f GROUP BY f.Category'
query = 'SELECT Category, (TotalValue * 100 / (SELECT SUM(TotalValue) FROM (SELECT Category, Total_Value AS \'TotalValue\' FROM (SELECT f.Category, SUM(f.Shares * f.ClosePrice) AS \'Total_Value\' FROM FinancialAssets f GROUP BY f.Category UNION SELECT \'Real Estate\' AS \'Category\', SUM(r.ClosePrice) AS \'Total_Value\' FROM RealEstates r) GROUP BY Category))) AS \'Percentage\' FROM (SELECT Category, Total_Value AS \'TotalValue\' FROM (SELECT f.Category, SUM(f.Shares * f.ClosePrice) AS \'Total_Value\' FROM FinancialAssets f GROUP BY f.Category UNION SELECT \'Real Estate\' AS \'Category\', SUM(r.ClosePrice) AS \'Total_Value\' FROM RealEstates r) GROUP BY Category) GROUP BY Category ORDER BY TotalValue DESC'
df = db.execute_query(query, df_format=True)
# sp_df = sp_df.set_index('Category')
print(df)
print(df.to_dict('tight'))

# analytics = DA(path_str)

# print(analytics.get_fa_results(db.get_data(table_name = 'FinancialAssets', df_format = True)))