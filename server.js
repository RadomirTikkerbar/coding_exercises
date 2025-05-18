const express = require('express');         
const fs = require('fs');                   
const csv = require('csv-parser');          

const app = express();                      
const PORT = 3000;                          

let spareParts = [];                        

fs.createReadStream('LE.csv')
  .pipe(csv({ separator: '\t' })) 
  .on('data', (row) => {
    spareParts.push(row);
  })
  .on('end', () => {
    console.log('CSV file successfully loaded into memory.');
  });


function filterAndPaginate(data, query) {
  const {
    serial_number,
    part_name,
    sort_by = 'price',                      
    sort_order = 'asc',                     
    page = 1,                               
    page_size = 10                          
  } = query;

  let result = data;

  
  if (serial_number) {
    result = result.filter(item =>
      item.serial_number?.toLowerCase().includes(serial_number.toLowerCase())
    );
  }

  
  if (part_name) {
    result = result.filter(item =>
      item.part_name?.toLowerCase().includes(part_name.toLowerCase())
    );
  }

  
  if (sort_by && result.length > 0 && sort_by in result[0]) {
    result = result.sort((a, b) => {
      const aVal = a[sort_by];
      const bVal = b[sort_by];

      
      if (!isNaN(aVal) && !isNaN(bVal)) {
        return sort_order === 'desc' ? bVal - aVal : aVal - bVal;
      } else {
        return sort_order === 'desc'
          ? bVal.toString().localeCompare(aVal)
          : aVal.toString().localeCompare(bVal);
      }
    });
  }


  const start = (page - 1) * page_size;
  const end = start + Number(page_size);
  const paginated = result.slice(start, end);

  return paginated;
}


app.get('/spare-parts', (req, res) => {
  const result = filterAndPaginate(spareParts, req.query);  
  res.json(result);                                          
});

app.get('/', (req, res) => {
  res.send('Welcome to Spare Parts API. Use /spare-parts to get data.');
});

app.get('/spare-parts', (req, res) => {
  res.json(spareParts);
});

app.listen(PORT, () => {
  console.log(`Server running at http://localhost:${PORT}`);
});
