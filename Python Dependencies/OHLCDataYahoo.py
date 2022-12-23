import os
import numpy as np
import pandas as pd
import yfinance as yf
from requests import Session
from io import StringIO

#NOTE: For os and glob, use '\\', for rest use '/' when handling directories
class OHLCDataYahoo:
    #data members
    __cwd = os.getcwd()
    __start_date = '2005-06-28'
    __end_date = '2022-07-27'
    __interval = '1d'

    __tickers_list = []
    __stock_basket = {}
    __stock_basket_sectors = {}

    __benchmark_indices = {
            'NIFTY50':'^NSEI', 
            'NIFTY500':'^CRSLDX', 
            'NIFTYBANK':'^NSEBANK',
            'NIFTYIT':'^CNXIT',
            'NIFTYHEALTHCARE':'NIFTY_HEALTHCARE.NS',
            'NIFTYFINSERVICE':'NIFTY_FIN_SERVICE.NS',
            'NIFTYAUTO':'^CNXAUTO',
            'NIFTYPHARMA':'^CNXPHARMA',
            'NIFTYFMCG':'^CNXFMCG',
            'NIFTYMEDIA':'^CNXMEDIA',
            'NIFTYMETAL':'^CNXMETAL',
            'NIFTYREALTY':'^CNXREALTY'
        }

    __url_dict = {
            'NIFTY50':'https://archives.nseindia.com/content/indices/ind_nifty50list.csv',
            'NIFTY500':'https://archives.nseindia.com/content/indices/ind_nifty500list.csv',
            'NIFTYBANK':'https://www.niftyindices.com/IndexConstituent/ind_niftybanklist.csv',
            'NIFTYIT':'https://www.niftyindices.com/IndexConstituent/ind_niftyitlist.csv',
            'NIFTYHEALTHCARE':'https://www.niftyindices.com/IndexConstituent/ind_niftyhealthcarelist.csv',
            'NIFTYFINSERVICE':'https://www.niftyindices.com/IndexConstituent/ind_niftyfinancelist.csv',
            'NIFTYAUTO':'https://www.niftyindices.com/IndexConstituent/ind_niftyautolist.csv',
            'NIFTYPHARMA':'https://www.niftyindices.com/IndexConstituent/ind_niftypharmalist.csv',
            'NIFTYFMCG':'https://www.niftyindices.com/IndexConstituent/ind_niftyfmcglist.csv',
            'NIFTYMEDIA':'https://www.niftyindices.com/IndexConstituent/ind_niftymedialist.csv',
            'NIFTYMETAL':'https://www.niftyindices.com/IndexConstituent/ind_niftymetallist.csv',
            'NIFTYREALTY':'https://www.niftyindices.com/IndexConstituent/ind_niftyrealtylist.csv'
        }


    def __init__(self, start_date, end_date, interval):
        self.__start_date = start_date
        self.__end_date = end_date
        self.__interval = interval

    def __process_dataframe(self, df, replace_close):
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

    def get_tickers_list(self):
        return self.__tickers_list
    
    def get_stock_basket(self):
        return self.__stock_basket

    def get_stock_basket_sectors(self):
        return self.__stock_basket_sectors

    def file_count(self, save_location):
        count = 0
        for file in os.listdir(self.__cwd + '\\' + save_location):
            count += 1
        return count

    def load_tickers(self, index = 'NIFTY50'):
        if index not in self.__url_dict.keys():
            print(f'{index} not recognized.')
            return
        
        try:
            session = Session()
            # Emulate browser
            session.headers.update({'user-agent': "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.61 Safari/537.36"})
            # Get the cookies from the main page (will update automatically in headers)
            session.get('https://www.nseindia.com/')
            # Get the API data
            data = session.get(self.__url_dict[index]).text
            data = StringIO(data)
            df = pd.read_csv(data, sep = ',')
            n = df.shape[0]
            self.__tickers_list.clear()
            for i in range(n):
                self.__tickers_list.append(df['Symbol'][i] + '.NS')
                self.__stock_basket_sectors[df['Symbol'][i] + '.NS'] = df['Industry'][i]
        except Exception as e:
            print(e)

    def get_multiple_data(self, save = True, save_location = 'Stocks Data/', replace_close = False):
        for ticker in self.__tickers_list:
            try:
                df = self.get_single_data(ticker, save, save_location, replace_close)
                if not df.empty:
                    self.__stock_basket[ticker] = df.copy()
            except:
                pass  
        self.__tickers_list = self.__stock_basket.keys() 

    #can be used to download a single stock's data, could also be a market index
    def get_single_data(self, ticker, save = True, save_location = 'Stocks Data/', replace_close = False):
        if ticker in self.__benchmark_indices.keys():
            ticker = self.__benchmark_indices[ticker]
        filename = save_location + ticker + self.__start_date + self.__end_date + self.__interval + '.csv'
        df = pd.DataFrame()
        try:
            df = pd.read_csv(filename)
        except FileNotFoundError:
            try:
                df = self.__download_single_data(ticker, save, save_location, replace_close)
            except:
                pass
        if not df.empty:
            df = self.__process_dataframe(df, replace_close)
        return df

    def __download_single_data(self, ticker, save = True, save_location = 'Stocks Data/', replace_close = False):
        filename = save_location + ticker + self.__start_date + self.__end_date + self.__interval + '.csv'
        filename_db = filename.replace(save_location, '')
        filename_db = filename_db.replace('.csv', '')
        data_found = True
        df = pd.DataFrame()
        try: 
            df = yf.download(
                                ticker,
                                start = self.__start_date,
                                end = self.__end_date,
                                interval = self.__interval
                            )
            df = self.__process_dataframe(df, False)
        except:
            data_found = False
        if save and data_found:
            save_df = df.copy()
            save_df = df.reset_index()
            save_df.to_csv(filename, index = False)
        return df