import forge from 'node-forge';

export namespace Certificate {
	export function generateX509(altNames: any[]) {
		const issuer = [
			{ name: 'commonName', value: 'Erkunden' },
			{ name: 'organizationName', value: 'Erkunden' },
			{ name: 'organizationalUnitName', value: 'Erkunden' }
		]
		const certificateExtensions = [
			{ name: 'basicConstraints', cA: true },
			{ name: 'keyUsage', keyCertSign: true, digitalSignature: true, nonRepudiation: true, keyEncipherment: true, dataEncipherment: true },
			{ name: 'extKeyUsage', serverAuth: true, clientAuth: true, codeSigning: true, emailProtection: true, timeStamping: true },
			{ name: 'nsCertType', client: true, server: true, email: true, objsign: true, sslCA: true, emailCA: true, objCA: true },
			{ name: 'subjectAltName', altNames },
			{ name: 'subjectKeyIdentifier' }
		]
		const keys = forge.pki.rsa.generateKeyPair(2048)
		const cert = forge.pki.createCertificate()
		cert.validity.notBefore = new Date()
		cert.validity.notAfter = new Date()
		cert.validity.notAfter.setFullYear(cert.validity.notBefore.getFullYear() + 1)
		cert.publicKey = keys.publicKey
		cert.setSubject(issuer)
		cert.setIssuer(issuer)
		cert.setExtensions(certificateExtensions)
		cert.sign(keys.privateKey)
		return {
			key: forge.pki.privateKeyToPem(keys.privateKey),
			cert: forge.pki.certificateToPem(cert)
		}
	}
}