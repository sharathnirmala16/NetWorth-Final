import os
import numpy as np
import pandas as pd
import yfinance
import plotly.express as px
from datetime import datetime as dt
pd.options.mode.chained_assignment = None
import seaborn as sns
sns.set()

class DataAnalytics:
    __base_path = 'C:\\Users\\shara\\AppData\\Local\\Packages\\b135f1fa-51ac-400e-bb0c-808ff7cf55d0_v6y00w1c367ya\\LocalState'
    
    def __init__(self, path) -> None:
        self.__base_path = path

    def __process_df(self, df, replace_close) -> pd.DataFrame():
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

    def get_fa_results(self, portfolio, title, subfolder, name) -> str:
        try:
            min_date = portfolio['OpenDate'].min()
            max_date = portfolio['CloseDate'].max()

            index_data = yfinance.download(tickers = ['^NSEI'], start = min_date, end = max_date, interval = '1d', progress = False)

            value_df = pd.DataFrame(columns = ['Date', 'Total Value'])
            value_df['Date'] = pd.Series(index_data.index)
            value_df = value_df.set_index('Date')
            value_df['Total Value'] = 0

            trackable_assets = dict(portfolio[(portfolio['Category'] == 'Stocks/ETF') | (portfolio['Category'] == 'Cryptocurrency')]['Name'])
            untrackable_assets = dict(portfolio[(portfolio['Category'] != 'Stocks/ETF') & (portfolio['Category'] != 'Cryptocurrency')]['Name'])

            trackable_assets_data = {}
            untrackable_assets_cagr = {}

            #trackable assets
            for key in trackable_assets.copy():
                try:
                    df = yfinance.download(
                                            trackable_assets[key],
                                            start = portfolio.loc[key]['OpenDate'],
                                            end = portfolio.loc[key]['CloseDate'],
                                            interval='1d',
                                            progress=False,
                                            threads=False
                                        )
                    if not df.empty:
                        trackable_assets_data[key] = self.__process_df(df, replace_close=False)['close']
                    else:
                        raise Exception('Empty Dataframe')
                except Exception as ex:
                    print(ex)
                    untrackable_assets[key] = trackable_assets[key]
                    trackable_assets.pop(key)


            value_ser = pd.Series(index = value_df.index, dtype = np.float64()).fillna(0)
            for date, row in value_df.iterrows(): 
                for key in trackable_assets_data.keys():
                    data = trackable_assets_data[key]
                    try:
                        value_ser[date]
                        value_ser[date] += portfolio.loc[key]['Shares'] * data.loc[date]['close']
                    except:
                        continue
                        
            value_df['Total Value'] = value_ser
            value_df = value_df[value_df['Total Value'] != 0]
            value_df = value_df.reset_index()

            if not os.path.exists(self.__base_path.replace('\\', '/') + f'/{subfolder}'):
                os.mkdir(self.__base_path.replace('\\', '/') + f'/{subfolder}')
                
            final_loc = self.__base_path.replace('\\', '/') + f'/{subfolder}/{name}'
            if os.path.exists(final_loc):
                os.remove(final_loc)

            fig = px.line(value_df, x = 'Date', y = 'Total Value', title = title)
            fig.write_image(final_loc)
            return final_loc
        except Exception as ex:
            print(ex)
            return 'FAIL'

    def pie_chart(self, df, X_label, Y_label, title, subfolder, name) -> str:
        try:
            if not os.path.exists(self.__base_path.replace('\\', '/') + f'/{subfolder}'):
                os.mkdir(self.__base_path.replace('\\', '/') + f'/{subfolder}')
                
            final_loc = self.__base_path.replace('\\', '/') + f'/{subfolder}/{name}'
            if os.path.exists(final_loc):
                os.remove(final_loc)
            fig = px.pie(df, values=Y_label, names=X_label, title=title)
            fig.write_image(final_loc)
            return final_loc
        except Exception as ex:
            print(ex)
            return 'FAIL'

    def bar_chart(self, df, X_label, Y_label, color_label, title, subfolder, name) -> str:
        try:
            if not os.path.exists(self.__base_path.replace('\\', '/') + f'/{subfolder}'):
                os.mkdir(self.__base_path.replace('\\', '/') + f'/{subfolder}')
                
            final_loc = self.__base_path.replace('\\', '/') + f'/{subfolder}/{name}'
            if os.path.exists(final_loc):
                os.remove(final_loc)
            fig = px.bar(df, y=Y_label, x=X_label, color=color_label, title=title)
            fig.write_image(final_loc)
            return final_loc
        except Exception as ex:
            print(ex)
            return 'FAIL'
        


        