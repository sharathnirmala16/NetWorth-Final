import os
from flask import Flask, jsonify, request
from waitress import serve
from SqliteDatabaseManager import SqliteDatabaseManager as dbms
from OHLCDataYahoo import OHLCDataYahoo as DataClass
from datetime import datetime as dt
from dateutil.relativedelta import relativedelta
from DataTools import DataAnalytics
import yfinance
import warnings
warnings.filterwarnings("ignore")
app = Flask(__name__)

path_str = 'C:\\Users\\shara\\AppData\\Local\\Packages\\b135f1fa-51ac-400e-bb0c-808ff7cf55d0_v6y00w1c367ya\\LocalState\\ApplicationDB.db'
folder_path = 'C:\\Users\\shara\\AppData\\Local\\Packages\\b135f1fa-51ac-400e-bb0c-808ff7cf55d0_v6y00w1c367ya\\LocalState'

@app.route('/start_test', methods = ['GET'])
def test_func() -> dict:
    return {'Server is live:': True}

@app.route('/DBpath', methods = ['POST'])
def db_path_change() -> dict:
    global path_str
    global folder_path
    request_dict = dict(request.json)
    path_str = request_dict['path']
    folder_path = request_dict['folderpath']
    response_dict = { 'exists': True }
    if not os.path.exists(path_str):
        response_dict['exists'] = False
    return response_dict

@app.route('/table', methods = ['POST'])
def financial_assets_table() -> dict:
    requested_table = dict(request.json)
    table_name = requested_table['table name']
    db_object = dbms(path_str)
    return db_object.get_data(table_name)

@app.route('/customquery', methods = ['POST'])
def query_db() -> dict:
    requested_table = dict(request.json)
    query = requested_table['query']
    db_object = dbms(path_str)
    return db_object.execute_query(query)

@app.route('/colquery', methods = ['POST'])
def col_query() -> dict:
    json_query = dict(request.json)
    query = json_query['query']
    col_name = json_query['colname']
    db_object = dbms(path_str)
    return db_object.execute_col_query(query, col_name)

@app.route('/insertfinancialasset', methods = ['PUT'])
def insert_fa_row() -> dict:
    dict_data = dict(request.json)
    db_object = dbms(path_str)
    resp = db_object.insert_row('FinancialAssets', 'AssetID', dict_data)
    return {'response': resp}

@app.route('/deletefinancialasset', methods = ['PUT'])
def delete_fa_row() -> dict:
    dict_data = dict(request.json)
    db_object = dbms(path_str)
    resp = db_object.delete_row('FinancialAssets', 'AssetID', dict_data)
    return {'response': resp }

@app.route('/values/open', methods = ['PUT'])
def get_open_prices() -> dict:
    dict_data = dict(request.json)
    resp_dict = {}
    db_object = dbms(path_str)

    for key, value in dict_data.items():
        failed_tickers = []
        end_date = dt.strptime(value['date'], '%Y-%m-%d')
        end_date = end_date + relativedelta(days=5)
        try:
            df = yfinance.download(
                                    value['ticker'],
                                    start = value['date'],
                                    end=end_date.strftime('%Y-%m-%d'),
                                    interval='1d',
                                    progress=False,
                                    threads=False
                                  )
            if not df.empty:
                db_object.process_df(df, replace_close=False)
                resp_dict[key] = df.iloc[0]['close']
            else:
                raise Exception('Empty Dataframe')
        except Exception as ex:
            failed_tickers.append(key)

    for key, val in resp_dict.items():
        err = db_object.update_value('FinancialAssets', 'AssetID', 'OpenPrice', key, round(val, 2))
        if err < 0:
            failed_tickers.append(key)

    return { 'failed': failed_tickers }

@app.route('/values/close', methods = ['PUT'])
def get_close_prices() -> dict:
    dict_data = dict(request.json)
    resp_dict = {}
    db_object = dbms(path_str)

    for key, value in dict_data.items():
        failed_tickers = []
        start_date = dt.strptime(value['date'], '%Y-%m-%d')
        start_date = start_date - relativedelta(days=5)
        try:
            df = yfinance.download(
                                    value['ticker'],
                                    start = start_date.strftime('%Y-%m-%d'),
                                    end=value['date'],
                                    interval='1d',
                                    progress=False,
                                    threads=False
                                  )
            if not df.empty:
                db_object.process_df(df, replace_close=False)
                resp_dict[key] = df.iloc[df.shape[0] - 1]['close']
            else:
                raise Exception('Empty Dataframe')
        except Exception as ex:
            failed_tickers.append(key)

    for key, val in resp_dict.items():
        err = db_object.update_value('FinancialAssets', 'AssetID', 'ClosePrice', key, round(val, 2))
        if err < 0:
            failed_tickers.append(key)

    return { 'failed': failed_tickers }

@app.route('/inserttransaction', methods = ['PUT'])
def insert_tr_row() -> dict:
    dict_data = dict(request.json)
    db_object = dbms(path_str)
    resp = db_object.insert_row('Spending', 'TransactionID', dict_data)
    return {'response': resp }

@app.route('/deletetransaction', methods = ['PUT'])
def delete_tr_row() -> dict:
    dict_data = dict(request.json)
    db_object = dbms(path_str)
    resp = db_object.delete_row('Spending', 'TransactionID', dict_data)
    return {'response': resp }

@app.route('/insertestate', methods = ['PUT'])
def insert_re_row() -> dict:
    dict_data = dict(request.json)
    db_object = dbms(path_str)
    resp = db_object.insert_row('RealEstates', 'EstateID', dict_data)
    return {'response': resp }

@app.route('/deleteestate', methods = ['PUT'])
def delete_re_row() -> dict:
    dict_data = dict(request.json)
    db_object = dbms(path_str)
    resp = db_object.delete_row('RealEstates', 'EstateID', dict_data)
    return {'response': resp }

@app.route('/insertloan', methods = ['PUT'])
def insert_lb_row() -> dict:
    dict_data = dict(request.json)
    db_object = dbms(path_str)
    resp = db_object.insert_row('Liabilities', 'LoanID', dict_data)
    return {'response': resp }

@app.route('/deleteloan', methods = ['PUT'])
def delete_lb_row() -> dict:
    dict_data = dict(request.json)
    db_object = dbms(path_str)
    resp = db_object.delete_row('Liabilities', 'LoanID', dict_data)
    return {'response': resp }

@app.route('/pie', methods = ['PUT'])
def make_pie_chart() -> dict:
    rec_req = dict(request.json)
    query = rec_req['query']
    X_label = rec_req['X_label']
    Y_label = rec_req['Y_label']
    title = rec_req['title']
    subfolder = rec_req['subfolder']
    name = rec_req['name']
    db_object = dbms(path_str)
    df = db_object.execute_query(query, df_format = True)
    da = DataAnalytics(folder_path)
    pie_resp = da.pie_chart(df, X_label, Y_label, title, subfolder, name)
    return { 'response': pie_resp }

@app.route('/track', methods = ['PUT'])
def make_line_chart() -> dict:
    rec_req = dict(request.json)
    title = rec_req['title']
    subfolder = rec_req['subfolder']
    name = rec_req['name']
    db_object = dbms(path_str)
    portfolio = db_object.get_data('FinancialAssets', df_format=True, index_col='AssetID')
    da = DataAnalytics(folder_path)
    line_resp = da.get_fa_results(portfolio, title, subfolder, name)
    return { 'response': line_resp }

@app.route('/bar', methods = ['PUT'])
def make_bar_chart() -> dict:
    rec_req = dict(request.json)
    query = rec_req['query']
    X_label = rec_req['X_label']
    Y_label = rec_req['Y_label']
    color_label = rec_req['color_label']
    title = rec_req['title']
    subfolder = rec_req['subfolder']
    name = rec_req['name']
    db_object = dbms(path_str)
    df = db_object.execute_query(query, df_format = True)
    da = DataAnalytics(folder_path)
    bar_resp = da.bar_chart(df, X_label, Y_label, color_label, title, subfolder, name)
    return { 'response': bar_resp }




# @app.route('/assetsanalysis', methods = ['PUT'])
# def perform_asset_analysis() -> dict:

serve(app, host='0.0.0.0', port=8080)