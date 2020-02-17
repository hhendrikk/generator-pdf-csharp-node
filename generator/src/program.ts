import fs from 'fs';
import path from 'path';
const PdfPrinter = require('pdfmake');

export class Program {
  async run(args: string[]) {
    const order = this.getOrder(args);
    const requests = fs.readdirSync(`./${order}`);

    requests.forEach((request: string) => {
      const pdfFilename = path.join(__dirname, order, request.replace(/\.js/, '.pdf'));
      const docDefinition = require(path.join(__dirname, order, request));
      this.processPDF(pdfFilename, docDefinition);
      this.removeRequest(pdfFilename);
    });
  }
  private removeRequest(pdfFilename: string) {
    fs.unlinkSync(pdfFilename.replace(/\.pdf/, '.js'));
  }

  private processPDF(pdfFilename: string, docDefinition: Object) {
    const fonts = this.getFonts();
    const printer = new PdfPrinter(fonts);

    const pdfDoc = printer.createPdfKitDocument(docDefinition);
    pdfDoc.pipe(fs.createWriteStream(pdfFilename));
    pdfDoc.end();
  }

  private getFonts() {
    return {
      Roboto: {
        normal: 'fonts/Roboto-Regular.ttf',
        bold: 'fonts/Roboto-Medium.ttf',
        italics: 'fonts/Roboto-Italic.ttf',
        bolditalics: 'fonts/Roboto-MediumItalic.ttf'
      }
    };
  }

  private getOrder(args: string[]) {
    return args[2];
  }
}
