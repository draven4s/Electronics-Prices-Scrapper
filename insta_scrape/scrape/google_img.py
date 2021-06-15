#!/usr/bin/python
# -*- coding: utf-8 -*-

import sys
import codecs

import selenium
from selenium import webdriver
import time
import requests
import os
from PIL import Image
import io
import hashlib
import numpy as np

# All in same directory
DRIVER_PATH = os.path.abspath(os.path.dirname(__file__) + "/chromedriver.exe")

def fetch_image_urls_topocentras(query:str, max_links_to_fetch:int, wd:webdriver, shopas:str, sleep_between_interactions:int=1):
    #sys.stdout = codecs.getwriter("iso-8859-1")
    
    def scroll_to_end(wd):
        wd.execute_script("window.scrollTo(0, document.body.scrollHeight);")
        time.sleep(sleep_between_interactions)        
    queryamount = 0
    results_start_query = 0
    thumbnail_results = []
    image_urls = set()
    datasets = set()
    # build the topocentras
    while (queryamount < max_links_to_fetch):
        search_url = "https://www.topocentras.lt/catalogsearch/result/?q={q}"
        
            # load the page
        wd.get(search_url.format(q=query))

        image_count = 0
        results_start = 0
        error_clicks = 0
        print("Query: ")
            
        while (image_count < 1) & (error_clicks < 30): 
            scroll_to_end(wd)        
            # get all image thumbnail results
            thumbnail_results = wd.find_elements_by_css_selector("div.ProductGrid-productWrapper-1hm")
            number_results = len(thumbnail_results)
            for img in thumbnail_results[queryamount:max_links_to_fetch]:
                # try to click every thumbnail such that we can get the real image behind it
                print("Info Start")
                print("ItemPrice")
                if wd.find_elements_by_css_selector('div.Price-oldPrice-3co'):
                    print("OldPrice : " + img.find_element_by_css_selector("div.Price-oldPrice-3co").text)
                if wd.find_elements_by_css_selector('div.Price-price-27p'):
                    print("CurPrice : " + img.find_element_by_css_selector("div.Price-price-27p").text)
                print("EndOfItemPrice")
                try:
                    img.click()
                    time.sleep(sleep_between_interactions)
                    print("LinkToItem")
                    print(wd.current_url)
                    print("EndOfLinkToItem")
                except Exception:
                    error_clicks = error_clicks + 1
                    if(results_start < number_results):
                        continue
                    else:
                        break
                                
                results_start = results_start + 1
                actual_images = wd.find_elements_by_css_selector('img.carousel-thumbnailImage-3_O')
                data = wd.find_elements_by_css_selector('li.Parameters-parametersListItem-Y1Q')
                prekesinfo = []
                if (len(data) > 0):
                    for ddd in data:
                        test = ddd.find_elements_by_css_selector('div.Parameters-title-1bx')
                        stuff = ddd.find_elements_by_css_selector('div.richText-root-uzW')
                        bigChungus = np.column_stack((test,stuff))
                        for sss in bigChungus:
                            temp = sss[0].text
                            temp1 = sss[1].text
                            print(temp + " : " + temp1)
                            #print(wd.find_elements_by_css_selector('div.richText-root-uzW').get_attribute("innerHTML"))
                print("Info Close")
                print("links: ")
                for actual_image in actual_images:
                    if actual_image.get_attribute('src') and 'http' in actual_image.get_attribute('src'):
                        image_urls.add(actual_image.get_attribute('src'))
                        print(actual_image.get_attribute('src'))
                        print("\n")
                print("links close")
                image_count = len(image_urls)
                queryamount = queryamount + 1
                break
            print("Query close")
            print("EndOfQuery")
            results_start = len(thumbnail_results)
            results_start_query = len(thumbnail_results)
        
    return image_urls

def fetch_image_urls_avitela(query:str, max_links_to_fetch:int, wd:webdriver, shopas:str, sleep_between_interactions:int=1):
    #sys.stdout = codecs.getwriter("iso-8859-1")
    
    def scroll_to_end(wd):
        wd.execute_script("window.scrollTo(0, document.body.scrollHeight);")
        time.sleep(sleep_between_interactions)        
    queryamount = 0
    results_start_query = 0
    thumbnail_results = []
    image_urls = set()
    datasets = set()
    # build the avitela
    while (queryamount < max_links_to_fetch):
        search_url = "https://avitela.lt/paieska/{q}"
        
        # load the page
        wd.get(search_url.format(q=query))

        image_count = 0
        results_start = 0
        error_clicks = 0
        print("Query: ")
            
        while (image_count < 1) & (error_clicks < 30): 
            scroll_to_end(wd)     
            # get all image thumbnail results
            thumbnail_results = wd.find_elements_by_css_selector("div.col-6.col-md-4.col-lg-4")
            number_results = len(thumbnail_results)
            for img in thumbnail_results[queryamount:max_links_to_fetch]:
                # try to click every thumbnail such that we can get the real image behind it
                print("Info Start")
                print("ItemPrice")
                if img.find_elements_by_css_selector('span.price-old'):
                    print("OldPrice : " + img.find_element_by_css_selector("span.price-old").text)
                if img.find_elements_by_css_selector('span.price-new'):
                    print("CurPrice : " + img.find_element_by_css_selector("span.price-new").text)
                if img.find_elements_by_css_selector('span.price'):
                    print("CurPrice : " + img.find_element_by_css_selector("span.price").text)
                print("EndOfItemPrice")
                try:
                    img.click()
                    time.sleep(sleep_between_interactions)
                    print("LinkToItem")
                    print(wd.current_url)
                    print("EndOfLinkToItem")
                except Exception:
                    error_clicks = error_clicks + 1
                    if(results_start < number_results):
                        continue
                    else:
                        break
                                
                results_start = results_start + 1
                actual_images = wd.find_elements_by_css_selector('a.popup-image')
                data = wd.find_elements_by_css_selector('tbody')
                prekes_kodas = wd.find_element_by_id('pmodel')
                prekesinfo = []
                print("Prekės kodas " + prekes_kodas.text)
                if (len(data) > 0):
                    for ddd in data:
                        test = ddd.find_elements_by_css_selector('tr')
                        for sss in test:
                            print(sss.text)
                            #print(wd.find_elements_by_css_selector('div.richText-root-uzW').get_attribute("innerHTML"))
                print("Info Close")
                print("links: ")
                for actual_image in actual_images:
                    if actual_image.get_attribute('href') and 'http' in actual_image.get_attribute('href'):
                        image_urls.add(actual_image.get_attribute('href'))
                        print(actual_image.get_attribute('href'))
                        print("\n")
                print("links close")
                image_count = len(image_urls)
                queryamount = queryamount + 1
                break
            print("Query close")
            print("EndOfQuery")
            results_start = len(thumbnail_results)
            results_start_query = len(thumbnail_results)
        
    return image_urls

def persist_image(folder_path:str,file_name:str,url:str):
    try:
        image_content = requests.get(url).content

    except Exception as e:
        print(f"ERROR - Could not download {url} - {e}")

    try:
        image_file = io.BytesIO(image_content)
        image = Image.open(image_file).convert('RGB')
        folder_path = os.path.join(folder_path,file_name)
        if os.path.exists(folder_path):
            file_path = os.path.join(folder_path,hashlib.sha1(image_content).hexdigest()[:10] + '.jpg')
        else:
            os.mkdir(folder_path)
            file_path = os.path.join(folder_path,hashlib.sha1(image_content).hexdigest()[:10] + '.jpg')
        with open(file_path, 'wb') as f:
            image.save(f, "JPEG", quality=85)
        print(f"SUCCESS - saved {url} - as {file_path}")
    except Exception as e:
        print(f"ERROR - Could not save {url} - {e}")

if __name__ == '__main__':
    sys.stdout.reconfigure(encoding='utf-8')
    wd = webdriver.Chrome(executable_path=DRIVER_PATH)
    queries = [sys.argv[1]] #change your set of queries here
    for query in queries:
        if sys.argv[3] == "Topocentras":
            wd.get('https://www.topocentras.lt/')
            wd.find_element_by_css_selector('.header-baseIcon-267.header-searchIcon-2YD').click()
            search_box = wd.find_element_by_css_selector('input.searchBar-input-3MY')
            search_box.send_keys(query)
            links = fetch_image_urls_topocentras(query,int(sys.argv[2]),wd,sys.argv[3]) # 200 denotes no. of images you want to download
            images_path = 'dataset/'
        elif sys.argv[3] == "Avitela":
            wd.get('https://www.avitela.lt/')
            search_box = wd.find_element_by_css_selector('input.input-block-level.search-query.tt-input')
            search_box.send_keys(query)
            links = fetch_image_urls_avitela(query,int(sys.argv[2]),wd,sys.argv[3]) # 200 denotes no. of images you want to download
            images_path = 'dataset/'

    wd.quit()
