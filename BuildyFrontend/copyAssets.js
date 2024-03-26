const fs = require("fs");
const path = require("path");

const sourcePath = path.join(
  __dirname,
  "src/assets/images/marker-icon-completed.png"
);
const destinationPath = path.join(
  __dirname,
  "node_modules/leaflet/dist/images/marker-icon-completed.png"
);

fs.copyFile(sourcePath, destinationPath, (err) => {
  if (err) throw err;
});
